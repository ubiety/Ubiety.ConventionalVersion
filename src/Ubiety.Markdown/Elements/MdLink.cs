namespace Ubiety.Markdown.Elements
{
    public class MdLink : IMarkdown
    {
        public readonly string _text;
        public readonly string _url;

        public MdLink(string text, string url)
        {
            _text = text;
            _url = url;
        }

        public string GetValue()
        {
            return $"[{_text}]({_url})";
        }

        public static implicit operator string(MdLink link)
        {
            return link.ToString();
        }

        public override string ToString()
        {
            return GetValue();
        }
    }
}