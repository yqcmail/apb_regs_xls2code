namespace RegGenerator
{
    public class FieldInfo
    {
        public long Id { get; set; }
        public long RegisterId { get; set; }
        public string Name { get; set; }
        public int LsbPos { get; set; }
        public int Width { get; set; }
        public string Access { get; set; }
        public long Reset { get; set; }
        public string Description { get; set; }
    }
}
