using System;
using System.IO;

namespace RegGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 2 && args[0] == "--generate-sample-db")
            {
                SampleDbGenerator.Create(args[1]);
                Console.WriteLine($"Sample database created at '{args[1]}'");
                return;
            }

            if (args.Length < 3)
            {
                PrintUsage();
                return;
            }

            string dbPath = args[0];
            string verilogPath = args[1];
            string rdlPath = args[2];
            string xlsPath = args.Length > 3 ? args[3] : null;


            Console.WriteLine($"Loading data from '{dbPath}'...");
            var registers = Database.LoadRegisters(dbPath);

            if (registers == null || registers.Count == 0)
            {
                Console.WriteLine("No registers loaded. Exiting.");
                return;
            }
            Console.WriteLine($"{registers.Count} registers loaded successfully.");

            Console.WriteLine($"Generating Verilog file at '{verilogPath}'...");
            try
            {
                string verilogContent = VerilogGenerator.Generate(registers);
                File.WriteAllText(verilogPath, verilogContent);
                Console.WriteLine("Verilog file generated successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating Verilog file: {ex.Message}");
            }

            Console.WriteLine($"Generating RDL file at '{rdlPath}'...");
            try
            {
                string rdlContent = RdlGenerator.Generate(registers);
                File.WriteAllText(rdlPath, rdlContent);
                Console.WriteLine("RDL file generated successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating RDL file: {ex.Message}");
            }

            if (!string.IsNullOrEmpty(xlsPath))
            {
                Console.WriteLine($"Generating XLS file at '{xlsPath}'...");
                try
                {
                    XlsGenerator.Generate(registers, xlsPath);
                    Console.WriteLine("XLS file generated successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error generating XLS file: {ex.Message}");
                }
            }
        }

        static void PrintUsage()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("  RegGenerator <db_path> <verilog_out_path> <rdl_out_path> [xls_out_path]");
            Console.WriteLine();
            Console.WriteLine("Commands:");
            Console.WriteLine("  --generate-sample-db <output_db_path>   Creates a sample SQLite database with example data.");
        }
    }
}
