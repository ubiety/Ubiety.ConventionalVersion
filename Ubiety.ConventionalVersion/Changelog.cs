using System;
using System.IO;
using LibGit2Sharp;
using Ubiety.Markdown;
using Ubiety.Markdown.Elements;

namespace Ubiety.ConventionalVersion
{
    public class Changelog
    {
        private FileInfo _changelogFile;
        private MarkdownDocument _changelog;

        private Changelog(FileInfo changelogFile)
        {
            _changelogFile = changelogFile;
            _changelog = new MarkdownDocument();
        }

        public string UpdateChangelog(Project project, Repository repository)
        {
            var currentDate = DateTimeOffset.Now;
            var origin = repository.Network.Remotes["origin"];

            _changelog.AddElement(new MdHeader("Change Log", HeaderWeight.One));
            _changelog.AddString($"All notable changes to this project will be documented in this file. See {new MdLink(origin.Name, origin.Url)} for commit guidelines.");

            return _changelog;
        }

        public static Changelog DiscoverChangelog(string workingDirectory)
        {
            var changelogFile = new FileInfo(Path.Combine(workingDirectory, "CHANGELOG.md"));

            return new Changelog(changelogFile);
        }
    }
}
