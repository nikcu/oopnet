namespace Utils
{
    public class TranslationOption
    {
        public required string Label { get; set; }
        public required string Value { get; set; }

        public static readonly string DisplayMember = "Label";
        public static readonly string ValueMember = "Value";
    }
}
