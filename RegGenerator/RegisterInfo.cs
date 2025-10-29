using System.Collections.Generic;

namespace RegGenerator
{
    public class RegisterInfo
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long Offset { get; set; }
        public int Size { get; set; }
        public string Description { get; set; }
        public List<FieldInfo> Fields { get; set; }

        public RegisterInfo()
        {
            Fields = new List<FieldInfo>();
        }
    }
}
