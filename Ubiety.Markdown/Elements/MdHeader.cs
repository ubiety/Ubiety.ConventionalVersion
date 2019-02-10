namespace Ubiety.Markdown.Elements
{
    public class MdHeader : IMarkdown
    {
        public enum HeaderWeight
        {
            One,
            Two,
            Three,
            Four,
            Five,
            Six
        }

        private readonly string _text;
        private readonly HeaderWeight _weight;

        public MdHeader(string text, HeaderWeight weight)
        {
            _text = text;
            _weight = weight;
        }

        public string GetValue()
        {
            switch (_weight)
            {
                case HeaderWeight.One:
                    return $"# {_text}";
                case HeaderWeight.Two:
                    return $"## {_text}";
                case HeaderWeight.Three:
                    return $"### {_text}";
                case HeaderWeight.Four:
                    return $"#### {_text}";
                case HeaderWeight.Five:
                    return $"##### {_text}";
                case HeaderWeight.Six:
                    return $"###### {_text}";
                default:
                    return _text;
            }
        }

        public static implicit operator string(MdHeader header)
        {
            return header.GetValue();
        }
    }
}
