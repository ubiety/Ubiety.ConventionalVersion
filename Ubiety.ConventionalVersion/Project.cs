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

        public ProjectVersion Version { get; }

        public IEnumerable<Commit> Commits { get; private set; }

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
            Commits = repository.GetCommitsSinceLastVersion(versionTag);

            var isMaster = repository.Head.FriendlyName == "master";

            var conventionalCommits = CommitParser.Parse(Commits);
            var incrementStrategy = VersionIncrementStrategy.Create(conventionalCommits, isMaster);

            return incrementStrategy.NextVersion(Version);
        }

        public void SetVersion(ProjectVersion nextVersion)
        {
            XDocument document = XDocument.Load(File);
            var versionElement = document.XPathSelectElement(versionXPath);
            versionElement.Value = nextVersion;
            document.Save(File);
        }

        public static Project Create(string projectFile)
        {
            return new Project(projectFile, GetVersion(projectFile));
        }
    }
}
