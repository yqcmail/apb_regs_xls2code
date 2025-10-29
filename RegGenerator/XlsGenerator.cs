using System.Collections.Generic;
using System.IO;
using OfficeOpenXml;

namespace RegGenerator
{
    public class XlsGenerator
    {
        public static void Generate(List<RegisterInfo> registers, string filePath)
        {
            // EPPlus requires a license context in .NET Core and later, but is fine in .NET Framework
            // ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Registers");

                // Header
                worksheet.Cells[1, 1].Value = "Reg_Name";
                worksheet.Cells[1, 2].Value = "Reg_Offset";
                worksheet.Cells[1, 3].Value = "Field_Name";
                worksheet.Cells[1, 4].Value = "Field_Lsb";
                worksheet.Cells[1, 5].Value = "Field_Width";
                worksheet.Cells[1, 6].Value = "Access";
                worksheet.Cells[1, 7].Value = "Reset";
                worksheet.Cells[1, 8].Value = "Description";

                int row = 2;
                foreach (var reg in registers)
                {
                    worksheet.Cells[row, 1].Value = reg.Name;
                    worksheet.Cells[row, 2].Value = $"0x{reg.Offset:X}";
                    worksheet.Cells[row, 8].Value = reg.Description;

                    // Style for the register row
                    worksheet.Cells[row, 1, row, 8].Style.Font.Bold = true;

                    row++;

                    foreach (var field in reg.Fields)
                    {
                        worksheet.Cells[row, 3].Value = field.Name;
                        worksheet.Cells[row, 4].Value = field.LsbPos;
                        worksheet.Cells[row, 5].Value = field.Width;
                        worksheet.Cells[row, 6].Value = field.Access;
                        worksheet.Cells[row, 7].Value = field.Reset;
                        worksheet.Cells[row, 8].Value = field.Description;
                        row++;
                    }
                }

                // Auto-fit columns
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                File.WriteAllBytes(filePath, package.GetAsByteArray());
            }
        }
    }
}
