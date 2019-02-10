namespace Ubiety.Markdown.Elements
{
    public class MdListItem : IMarkdown
    {
        private readonly string _text;

        public MdListItem(string text)
        {
            _text = text;
        }

        public string GetValue()
        {
            return $"  * {_text}";
        }

        public static implicit operator string(MdListItem listItem)
        {
            return listItem.ToString();
        }

        public override string ToString()
        {
            return GetValue();
        }
    }
}
