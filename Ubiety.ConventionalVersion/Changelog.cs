using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using LibGit2Sharp;
using Ubiety.ConventionalVersion.Commits;
using Ubiety.ConventionalVersion.Extensions;
using Ubiety.Markdown;
using Ubiety.Markdown.Elements;

namespace Ubiety.ConventionalVersion
{
    public class Changelog
    {
        private readonly FileInfo _changelogFile;

        private Changelog(FileInfo changelogFile)
        {
            _changelogFile = changelogFile;
        }

        public string FilePath { get => _changelogFile.FullName; }

        public string UpdateChangelog(Project project, Repository repository)
        {
            var currentDate = DateTimeOffset.Now;
            var changelog = new MarkdownDocument();

            var gitUrl = new GitUrl(repository.Network.Remotes["origin"].Url);

            changelog.AddElement(new MdHeader("Change Log", HeaderWeight.One));
            changelog.AddNewLines();
            changelog.AddText($"All notable changes to this project will be documented in this file. See {new MdLink("Conventional Commits", "https://conventionalcommits.org")} for commit guidelines.");
            changelog.AddElement(new MdRule());
            changelog.AddNewLines(2);

            changelog.AddText($"<a name=\"{project.Version}\"></a>");
            changelog.AddElement(new MdHeader(
                $"{new MdLink($"{project.Version}", $"{gitUrl.CompareUrl}/{project.Version.PreviousTag}...{project.Version.Tag}/")} ({currentDate.Date.ToString("yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo)})",
                HeaderWeight.Two));
            changelog.AddNewLines();

            if (project.BugCommits.IsAny())
            {
                AddCommits("Bug Fixes", project.BugCommits, changelog);
                changelog.AddNewLines();
            }

            if (project.FeatureCommits.IsAny())
            {
                AddCommits("Features", project.FeatureCommits, changelog);
                changelog.AddNewLines();
            }

            if (project.BreakingCommits.IsAny())
            {
                AddCommits("Breaking Changes", project.BreakingCommits, changelog);
                changelog.AddNewLines();
            }

            return changelog;
        }

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

        public static Changelog DiscoverChangelog(string workingDirectory)
        {
            var changelogFile = new FileInfo(Path.Combine(workingDirectory, "CHANGELOG.md"));

            return new Changelog(changelogFile);
        }

        private void AddCommits(string header, IEnumerable<ConventionalCommit> commits, MarkdownDocument changelog)
        {
            changelog.AddElement(new MdHeader(header, HeaderWeight.Three));
            changelog.AddNewLines();
            foreach (var commit in commits)
            {
                changelog.AddElement(new MdListItem(commit.Subject));
            }
        }
    }
}
