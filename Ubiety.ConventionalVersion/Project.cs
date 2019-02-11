using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using LibGit2Sharp;
using Ubiety.ConventionalVersion.Commits;
using Ubiety.ConventionalVersion.Extensions;

namespace Ubiety.ConventionalVersion
{
    public class Project
    {
        private const string versionXPath = "./Project/PropertyGroup/Version";

        private Project(string file, ProjectVersion version)
        {
            File = file;
            Version = version;
        }

        public string File { get; }

        public ProjectVersion Version { get; private set; }

        public IEnumerable<ConventionalCommit> Commits { get; private set; }

        public IEnumerable<ConventionalCommit> FeatureCommits { get => Commits.Where(commit => "feat".Equals(commit.Type, StringComparison.InvariantCultureIgnoreCase)); }
        
        public IEnumerable<ConventionalCommit> BugCommits { get => Commits.Where(commit => "fix".Equals(commit.Type, StringComparison.InvariantCultureIgnoreCase)); }

        public IEnumerable<ConventionalCommit> BreakingCommits { get => Commits.Where(commit => commit.Notes.Any(note => note.Title.Equals("BREAKING CHANGE", StringComparison.InvariantCulture))); }

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
            if (GetVersion(projectFile) == null)
            {
                return false;
            }

            return true;
        }

        public static ProjectVersion GetVersion(string projectFile)
        {
            XDocument document = XDocument.Load(projectFile);
            var versionElement = document.XPathSelectElement(versionXPath);

            if (versionElement is null)
            {
                return default;
            }

            return new ProjectVersion(versionElement.Value);
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
            XDocument document = XDocument.Load(File);
            var versionElement = document.XPathSelectElement(versionXPath);
            versionElement.Value = Version = nextVersion;
            document.Save(File);
        }

        public static Project Create(string projectFile)
        {
            return new Project(projectFile, GetVersion(projectFile));
        }
    }
}
