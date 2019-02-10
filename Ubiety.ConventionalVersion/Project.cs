using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using LibGit2Sharp;
using Ubiety.ConventionalVersion.Commits;
using Ubiety.ConventionalVersion.Extensions;
using Version = System.Version;

namespace Ubiety.ConventionalVersion
{
    public class Project
    {
        private const string versionXPath = "./Project/PropertyGroup/Version";

        private Project(string file, Version version)
        {
            File = file;
            Version = version;
        }

        public string File { get; }

        public Version Version { get; }

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

        public static Version GetVersion(string projectFile)
        {
            XDocument document = XDocument.Load(projectFile);
            var versionElement = document.XPathSelectElement(versionXPath);

            if (versionElement is null)
            {
                return default;
            }

            return new Version(versionElement.Value);
        }

        public Version GetNextVersion(Repository repository)
        {
            var versionTag = repository.GetVersionTag(Version);
            var commits = repository.GetCommitsSinceLastVersion(versionTag);

            var conventionalCommits = CommitParser.Parse(commits);
            var incrementStrategy = VersionIncrementStrategy.Create(conventionalCommits);

            return incrementStrategy.NextVersion(Version);
        }

        public void SetVersion(Version nextVersion)
        {
            XDocument document = XDocument.Load(File);
            var versionElement = document.XPathSelectElement(versionXPath);
            versionElement.Value = nextVersion.ToString();
            document.Save(File);
        }

        public static Project Create(string projectFile)
        {
            return new Project(projectFile, GetVersion(projectFile));
        }
    }
}
