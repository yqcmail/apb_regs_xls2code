using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace RegGenerator
{
    public class Database
    {
        public static List<RegisterInfo> LoadRegisters(string dbPath)
        {
            var registers = new Dictionary<long, RegisterInfo>();

            if (!System.IO.File.Exists(dbPath))
            {
                Console.WriteLine($"Error: Database file not found at '{dbPath}'");
                return new List<RegisterInfo>();
            }

            string connectionString = $"Data Source={dbPath};Version=3;";

            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    // First, read all registers
                    string regQuery = "SELECT id, name, offset, size, description FROM Registers;";
                    using (var command = new SQLiteCommand(regQuery, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var reg = new RegisterInfo
                            {
                                Id = (long)reader["id"],
                                Name = (string)reader["name"],
                                Offset = (long)reader["offset"],
                                Size = Convert.ToInt32(reader["size"]),
                                Description = reader["description"] as string ?? ""
                            };
                            registers.Add(reg.Id, reg);
                        }
                    }

                    // Then, read all fields and assign them to their respective registers
                    string fieldQuery = "SELECT id, register_id, name, lsb_pos, width, access, reset, description FROM Fields;";
                    using (var command = new SQLiteCommand(fieldQuery, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var field = new FieldInfo
                            {
                                Id = (long)reader["id"],
                                RegisterId = (long)reader["register_id"],
                                Name = (string)reader["name"],
                                LsbPos = Convert.ToInt32(reader["lsb_pos"]),
                                Width = Convert.ToInt32(reader["width"]),
                                Access = (string)reader["access"],
                                Reset = (long)reader["reset"],
                                Description = reader["description"] as string ?? ""
                            };

                            if (registers.ContainsKey(field.RegisterId))
                            {
                                registers[field.RegisterId].Fields.Add(field);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while accessing the database: {ex.Message}");
                return new List<RegisterInfo>();
            }

            return new List<RegisterInfo>(registers.Values);
        }
    }
}
