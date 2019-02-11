using System.Text.RegularExpressions;

namespace Ubiety.ConventionalVersion
{
    public class GitUrl
    {
        private readonly string _url;
        private readonly Regex urlRegex = new Regex("^(?<user>.*)\\@(?<server>.*)\\:(?<org>.*)\\/(?<repo>.*)\\.git$", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Singleline);

        public GitUrl(string url)
        {
            _url = url;
            var matches = urlRegex.Match(_url);

            if (matches.Success)
            {
                Host = matches.Groups["server"].Value;
                Organization = matches.Groups["org"].Value;
                Repository = matches.Groups["repo"].Value;
            }
        }

        public string Host { get; private set; }
        public string Organization { get; private set; }
        public string Repository { get; private set; }
        public string WebUrl { get => $"https://{Host}/{Organization}/{Repository}"; }
        public string CompareUrl { get => $"{WebUrl}/compare"; }
    }
}
