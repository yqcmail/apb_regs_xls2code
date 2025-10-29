using System.Collections.Generic;
using System.Text;

namespace RegGenerator
{
    public class RdlGenerator
    {
        public static string Generate(List<RegisterInfo> registers, string mapName = "my_reg_block")
        {
            var sb = new StringBuilder();

            sb.AppendLine($"addrmap {mapName} {{");
            sb.AppendLine($"    desc = \"{mapName} Register Map\";");
            sb.AppendLine();

            foreach (var reg in registers)
            {
                sb.AppendLine($"    reg {reg.Name} @0x{reg.Offset:X} {{");
                if (!string.IsNullOrEmpty(reg.Description))
                {
                    sb.AppendLine($"        desc = \"{reg.Description}\";");
                }

                foreach (var field in reg.Fields)
                {
                    sb.AppendLine($"        field {{");
                    sb.AppendLine($"            sw = {GetRdlAccess(field.Access)};");
                    if (!string.IsNullOrEmpty(field.Description))
                    {
                        sb.AppendLine($"            desc = \"{field.Description}\";");
                    }
                    sb.AppendLine($"            reset = {field.Reset};");
                    int msb = field.LsbPos + field.Width - 1;
                    sb.AppendLine($"        }} {field.Name}[{msb}:{field.LsbPos}];");
                }
                sb.AppendLine("    };");
                sb.AppendLine();
            }

            sb.AppendLine("};");

            return sb.ToString();
        }

        private static string GetRdlAccess(string access)
        {
            switch (access.ToUpper())
            {
                case "RW":
                    return "rw";
                case "RO":
                    return "r";
                case "WO":
                    return "w";
                case "W1C":
                    return "w1"; // Or handle more complex types if needed
                case "RC":
                     return "r"; // Or handle more complex types if needed
                default:
                    return "rw"; // Default
            }
        }
    }
}
