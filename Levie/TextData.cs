namespace Civ6TranslationToolWPF.Levie
{
    public class TextData
    {
        public int Index { get; set; }
        public string OriginalText { get; set; }
        public string TranslatedText { get; set; }
        public string Language { get; set; }
        public string Context { get; set; }
        public string Tag { get; set; }
        public bool IsHighlighted { get; set; } // Thuộc tính để xác định highlight
    }
}
