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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using LibGit2Sharp;
using Ubiety.VersionIt.Commits.Rules;

namespace Ubiety.ConventionalVersion.Commits
{
    /// <summary>
    ///     Git commit parser.
    /// </summary>
    public static class CommitParser
    {
        private static readonly Regex HeaderPattern =
            new Regex(
                "^(?<type>\\w*)(?:\\((?<scope>.*)\\))?: (?<subject>.*)$",
                RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Singleline);

        private static readonly string[] NoteKeywords = { "BREAKING CHANGE" };

        /// <summary>
        ///     Parse the commit.
        /// </summary>
        /// <param name="commit">Git commit to parse.</param>
        /// <returns>A new <see cref="ConventionalCommit"/> representing the git commit.</returns>
        public static ConventionalCommit Parse(Commit commit)
        {
            var conventionalCommit = new ConventionalCommit();

            var commitLines = commit
                .Message
                .Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None)
                .Select(line => line.Trim())
                .Where(line => !string.IsNullOrEmpty(line))
                .ToList();

            var header = commitLines.FirstOrDefault();

            if (string.IsNullOrEmpty(header))
            {
                return conventionalCommit;
            }

            var headerParts = HeaderPattern.Match(header);

            if (headerParts.Success)
            {
                conventionalCommit.Scope = headerParts.Groups["scope"].Value;
                conventionalCommit.Subject = headerParts.Groups["subject"].Value;

                conventionalCommit.Type = ConventionalRules
                    .Rules
                    .FirstOrDefault(rule => rule.Value.Type.Equals(
                        headerParts.Groups["type"].Value,
                        StringComparison.InvariantCulture)).Key;
            }
            else
            {
                conventionalCommit.Subject = header;
            }

            for (var i = 1; i < commitLines.Count(); i++)
            {
                foreach (var keyword in NoteKeywords)
                {
                    if (commitLines[i].StartsWith(keyword, StringComparison.InvariantCulture))
                    {
                        conventionalCommit.Notes.Add(new CommitNote
                        {
                            Title = keyword,
                            Text = commitLines[i].Substring($"{keyword}:".Length).TrimStart(),
                        });
                    }
                }
            }

            return conventionalCommit;
        }

        /// <summary>
        ///     Parse a number of git commits.
        /// </summary>
        /// <param name="commits">List of commits to parse.</param>
        /// <returns>List of <see cref="ConventionalCommit"/>.</returns>
        public static IEnumerable<ConventionalCommit> Parse(IEnumerable<Commit> commits)
        {
            return commits.Select(Parse).ToList();
        }
    }
}