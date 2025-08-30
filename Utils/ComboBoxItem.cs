namespace Utils
{
    public class ComboBoxItem
    {
        public required string Label { get; set; }
        public required string Value { get; set; }

        public const string DisplayMember = "Label";
        public const string ValueMember = "Value";
    }
}
