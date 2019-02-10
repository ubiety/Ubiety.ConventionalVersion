namespace Ubiety.Markdown.Elements
{
    public class MdRule : IMarkdown
    {
        public string GetValue()
        {
            return "---";
        }

        public static implicit operator string(MdRule rule)
        {
            return rule.GetValue();
        }
    }
}
