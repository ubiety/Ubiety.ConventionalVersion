using System.Text;

namespace Ubiety.Markdown
{
    public class MarkdownDocument
    {
        private readonly StringBuilder _document;

        public MarkdownDocument()
        {
            _document = new StringBuilder();
        }

        public void AddElement(IMarkdown element)
        {
            _document.Append(element);
            _document.AppendLine();
        }

        public void AddText(string value)
        {
            _document.AppendLine(value);
        }

        public void AddNewLines(int lines = 1)
        {
            for (int i = 0; i < lines; i++)
            {
                _document.AppendLine();
            }
        }

        public static implicit operator string(MarkdownDocument document)
        {
            return document._document.ToString();
        }
    }
}
