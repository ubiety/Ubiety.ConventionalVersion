using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using LibGit2Sharp;
using Ubiety.ConventionalVersion.Commits;
using Ubiety.ConventionalVersion.Extensions;
using Ubiety.VersionIt.Commits.Rules;

namespace Ubiety.ConventionalVersion
{
    public class Project
    {
        private const string VersionXPath = ".//PropertyGroup/Version";

        private Project(string file, ProjectVersion version)
        {
            File = file;
            Version = version;
        }

        public string File { get; }

        public ProjectVersion Version { get; set; }

        public IEnumerable<ConventionalCommit> Commits { get; private set; }

        public IEnumerable<ConventionalCommit> BreakingCommits => Commits.Where(commit =>
            commit.Notes.Any(note => note.Title.Equals("BREAKING CHANGE", StringComparison.InvariantCulture)));

        public static IEnumerable<Project> DiscoverProjects(string directory)
        {
            return Directory
                .GetFiles(directory, "*.csproj", SearchOption.AllDirectories)
                .Where(IsVersionable)
                .Select(Create)
                .ToList();
        }

        public static bool IsVersionable(string projectFile)
        {
            return GetVersion(projectFile) != null;
        }

        public static ProjectVersion GetVersion(string projectFile)
        {
            var document = XDocument.Load(projectFile);
            var versionElement = document.XPathSelectElement(VersionXPath);

            return versionElement is null ? default : new ProjectVersion(versionElement.Value);
        }

        public static Project Create(string projectFile)
        {
            return new Project(projectFile, GetVersion(projectFile));
        }

        public IEnumerable<ConventionalCommit> GetCommits(ConventionalTypes type)
        {
            return Commits.Where(commit => commit.Type == type);
        }

        public ProjectVersion GetNextVersion(Repository repository)
        {
            var versionTag = repository.GetVersionTag(Version);
            var commits = repository.GetCommitsSinceLastVersion(versionTag);

            var isMaster = repository.Head.FriendlyName == "master";

            Commits = CommitParser.Parse(commits);
            var incrementStrategy = VersionIncrementStrategy.Create(Commits, isMaster);

            return incrementStrategy.NextVersion(Version);
        }

        public void SetVersion(ProjectVersion nextVersion)
        {
            var document = XDocument.Load(File);
            var versionElement = document.XPathSelectElement(VersionXPath);
            versionElement.Value = nextVersion;
            document.Save(File);
        }
    }
}