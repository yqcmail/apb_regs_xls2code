using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RegGenerator
{
    public class VerilogGenerator
    {
        public static string Generate(List<RegisterInfo> registers, string moduleName = "my_reg_block")
        {
            var sb = new StringBuilder();

            // Module Header
            sb.AppendLine($"module {moduleName} (");
            sb.AppendLine("    // APB Interface");
            sb.AppendLine("    input           PCLK,");
            sb.AppendLine("    input           PRESETn,");
            sb.AppendLine("    input           PSEL,");
            sb.AppendLine("    input           PENABLE,");
            sb.AppendLine("    input   [31:0]  PADDR,");
            sb.AppendLine("    input           PWRITE,");
            sb.AppendLine("    input   [31:0]  PWDATA,");
            sb.AppendLine("    output  [31:0]  PRDATA,");
            sb.AppendLine("    output          PREADY");
            sb.AppendLine(");");
            sb.AppendLine();

            // Internal register declarations
            sb.AppendLine("    // Internal register declarations");
            foreach (var reg in registers)
            {
                sb.AppendLine($"    reg [{reg.Size - 1}:0] {reg.Name};");
            }
            sb.AppendLine();

            // Write Logic
            sb.AppendLine("    // Write Logic");
            sb.AppendLine("    always @(posedge PCLK or negedge PRESETn)");
            sb.AppendLine("    begin");
            sb.AppendLine("        if (!PRESETn)");
            sb.AppendLine("        begin");
            // Reset logic
            foreach (var reg in registers)
            {
                long resetValue = 0;
                foreach (var field in reg.Fields)
                {
                    resetValue |= field.Reset << field.LsbPos;
                }
                sb.AppendLine($"            {reg.Name} <= 32'h{resetValue:X};");
            }
            sb.AppendLine("        end");
            sb.AppendLine("        else if (PSEL && PENABLE && PWRITE)");
            sb.AppendLine("        begin");
            sb.AppendLine("            case (PADDR)");
            foreach (var reg in registers)
            {
                sb.AppendLine($"                32'h{reg.Offset:X}: begin");
                foreach (var field in reg.Fields.Where(f => f.Access.Contains("W")))
                {
                    int msb = field.LsbPos + field.Width - 1;
                    sb.AppendLine($"                    {reg.Name}[{msb}:{field.LsbPos}] <= PWDATA[{msb}:{field.LsbPos}];");
                }
                sb.AppendLine("                end");
            }
            sb.AppendLine("            endcase");
            sb.AppendLine("        end");
            sb.AppendLine("    end");
            sb.AppendLine();

            // Read Logic
            sb.AppendLine("    // Read Logic");
            sb.AppendLine("    reg [31:0] prdata_reg;");
            sb.AppendLine("    always @(*)");
            sb.AppendLine("    begin");
            sb.AppendLine("        case (PADDR)");
            foreach (var reg in registers)
            {
                sb.AppendLine($"            32'h{reg.Offset:X}: prdata_reg = {reg.Name};");
            }
            sb.AppendLine("            default: prdata_reg = 32'h0;");
            sb.AppendLine("        endcase");
            sb.AppendLine("    end");
            sb.AppendLine();
            sb.AppendLine("    assign PRDATA = prdata_reg;");
            sb.AppendLine("    assign PREADY = 1'b1;");
            sb.AppendLine();
            sb.AppendLine("endmodule");

            return sb.ToString();
        }
    }
}
