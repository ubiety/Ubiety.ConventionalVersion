/* Copyright 2019 Dieter Lunn
 *
 *   Licensed under the Apache License, Version 2.0 (the "License");
 *   you may not use this file except in compliance with the License.
 *   You may obtain a copy of the License at
 *
 *       http://www.apache.org/licenses/LICENSE-2.0
 *
 *   Unless required by applicable law or agreed to in writing, software
 *   distributed under the License is distributed on an "AS IS" BASIS,
 *   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *   See the License for the specific language governing permissions and
 *   limitations under the License.
 */

using System.Text.RegularExpressions;

namespace Ubiety.VersionIt
{
    /// <summary>
    ///     Git project URL.
    /// </summary>
    public class GitUrl
    {
        private readonly Regex _urlRegex = new Regex(
            "^(?<user>.*)\\@(?<server>.*)\\:(?<org>.*)\\/(?<repo>.*)\\.git$",
            RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Singleline);

        /// <summary>
        ///     Initializes a new instance of the <see cref="GitUrl"/> class.
        /// </summary>
        /// <param name="url">String of the git URL.</param>
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

        /// <summary>
        ///     Gets the URL host.
        /// </summary>
        public string Host { get; }

        /// <summary>
        ///     Gets the repository organization.
        /// </summary>
        public string Organization { get; }

        /// <summary>
        ///     Gets the repository name.
        /// </summary>
        public string Repository { get; }

        /// <summary>
        ///     Gets the web URL.
        /// </summary>
        public string WebUrl => $"https://{Host}/{Organization}/{Repository}";

        /// <summary>
        ///     Gets the project comparison URL.
        /// </summary>
        public string CompareUrl => $"{WebUrl}/compare";
    }
}