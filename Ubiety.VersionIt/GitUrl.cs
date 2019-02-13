using System.Text.RegularExpressions;

namespace Ubiety.ConventionalVersion
{
    public class GitUrl
    {
        private readonly Regex _urlRegex = new Regex(
            "^(?<user>.*)\\@(?<server>.*)\\:(?<org>.*)\\/(?<repo>.*)\\.git$",
            RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Singleline);

        public GitUrl(string url)
        {
            var matches = _urlRegex.Match(url);

            if (!matches.Success)
            {
                return;
            }

            Host = matches.Groups["server"].Value;
            Organization = matches.Groups["org"].Value;
            Repository = matches.Groups["repo"].Value;
        }

        public string Host { get; }

        public string Organization { get; }

        public string Repository { get; }

        public string WebUrl => $"https://{Host}/{Organization}/{Repository}";

        public string CompareUrl => $"{WebUrl}/compare";
    }
}