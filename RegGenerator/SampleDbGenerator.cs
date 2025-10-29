using System.Data.SQLite;

public class SampleDbGenerator
{
    public static void Create(string dbPath)
    {
        using (var connection = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
        {
            connection.Open();

            string createRegs = "CREATE TABLE Registers (id INTEGER PRIMARY KEY, name TEXT, offset INTEGER, size INTEGER, description TEXT);";
            string createFields = "CREATE TABLE Fields (id INTEGER PRIMARY KEY, register_id INTEGER, name TEXT, lsb_pos INTEGER, width INTEGER, access TEXT, reset INTEGER, description TEXT);";

            new SQLiteCommand(createRegs, connection).ExecuteNonQuery();
            new SQLiteCommand(createFields, connection).ExecuteNonQuery();

            string insertReg = "INSERT INTO Registers (id, name, offset, size, description) VALUES (1, 'CTRL_REG', 4, 32, 'Control Register');";
            new SQLiteCommand(insertReg, connection).ExecuteNonQuery();

            string insertFields = @"
                INSERT INTO Fields (register_id, name, lsb_pos, width, access, reset, description) VALUES
                (1, 'ENABLE', 0, 1, 'RW', 1, 'Enable bit'),
                (1, 'START', 1, 1, 'WO', 0, 'Start bit'),
                (1, 'INT_STATUS', 8, 4, 'RO', 0, 'Interrupt Status');
            ";
            new SQLiteCommand(insertFields, connection).ExecuteNonQuery();
        }
    }
}
