namespace Ubiety.Markdown.Elements
{
    public class MdBold : IMarkdown
    {
        private readonly string _text;

        public MdBold(string text)
        {
            _text = text;
        }

        public string GetValue()
        {
            return $"**{_text}**";
        }

        public static implicit operator string(MdBold bold)
        {
            return bold.ToString();
        }

        public override string ToString()
        {
            return GetValue();
        }
    }
}