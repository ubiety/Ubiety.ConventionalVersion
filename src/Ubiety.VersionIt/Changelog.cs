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
using System.Globalization;
using System.IO;
using LibGit2Sharp;
using Ubiety.VersionIt.Commits;
using Ubiety.VersionIt.Extensions;
using Ubiety.Markdown;
using Ubiety.Markdown.Elements;
using Ubiety.VersionIt.Commits.Rules;

namespace Ubiety.VersionIt
{
    /// <summary>
    ///     Project changelog.
    /// </summary>
    public class Changelog
    {
        private readonly FileInfo _changelogFile;

        private Changelog(FileInfo changelogFile)
        {
            _changelogFile = changelogFile;
        }

        /// <summary>
        ///     Gets the changelog file path.
        /// </summary>
        public string FilePath => _changelogFile.FullName;

        /// <summary>
        ///     Update the changelog.
        /// </summary>
        /// <param name="project">Project to update changelog for.</param>
        /// <param name="repository">Git repository of the project.</param>
        /// <returns>A string version of the new changelog.</returns>
        public static string UpdateChangelog(Project project, Repository repository)
        {
            var currentDate = DateTimeOffset.Now;
            var changelog = new MarkdownDocument();

            var gitUrl = new GitUrl(repository.Network.Remotes["origin"].Url);

            changelog.AddElement(new MdHeader("Change Log", HeaderWeight.One));
            changelog.AddNewLines();
            changelog.AddText(
                $"All notable changes to this project will be documented in this file. See {new MdLink("Conventional Commits", "https://conventionalcommits.org")} for commit guidelines.");
            changelog.AddElement(new MdRule());
            changelog.AddNewLines(2);

            changelog.AddText($"<a name=\"{project.Version}\"></a>");
            changelog.AddElement(new MdHeader(
                $"{new MdLink($"{project.Version}", $"{gitUrl.CompareUrl}/{(string.IsNullOrEmpty(project.Version.PreviousTag) ? "master" : project.Version.PreviousTag)}...{project.Version.Tag}/")} ({currentDate.Date.ToString("yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo)})",
                HeaderWeight.Two));
            changelog.AddNewLines();

            foreach (var rule in ConventionalRules.Rules)
            {
                var commits = project.GetCommits(rule.Key);
                if (commits.IsAny())
                {
                    AddCommits(rule.Value.Header, commits, changelog);
                }
            }

            if (project.BreakingCommits.IsAny())
            {
                AddCommits("Breaking Changes", project.BreakingCommits, changelog);
                changelog.AddNewLines();
            }

            return changelog;
        }

        /// <summary>
        ///     Discover the changelog file.
        /// </summary>
        /// <param name="workingDirectory">Directory to search.</param>
        /// <returns>A new <see cref="Changelog"/> instance.</returns>
        public static Changelog DiscoverChangelog(string workingDirectory)
        {
            var changelogFile = new FileInfo(Path.Combine(workingDirectory, "CHANGELOG.md"));

            return new Changelog(changelogFile);
        }

        /// <summary>
        ///     Write the changelog to a file.
        /// </summary>
        /// <param name="changelog">String of the changelog to write.</param>
        public void WriteFile(string changelog)
        {
            if (_changelogFile.Exists)
            {
                var currentChangelog = File.ReadAllText(_changelogFile.FullName);

                var firstVersionIndex = currentChangelog.IndexOf("##", StringComparison.InvariantCulture);

                if (firstVersionIndex >= 0)
                {
                    currentChangelog = currentChangelog.Substring(firstVersionIndex);
                }

                changelog += $"\n{currentChangelog}";
            }

            File.WriteAllText(_changelogFile.FullName, changelog);
        }

        private static void AddCommits(string header, IEnumerable<ConventionalCommit> commits, MarkdownDocument changelog)
        {
            changelog.AddElement(new MdHeader(header, HeaderWeight.Three));
            changelog.AddNewLines();
            foreach (var commit in commits)
            {
                changelog.AddElement(new MdListItem(commit.Subject));
            }

            changelog.AddNewLines();
        }
    }
}