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

        public IEnumerable<ConventionalCommit> BreakingCommits => Commits?.Where(commit =>
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
            var version = GetVersion(projectFile);
            return !(version is null);
        }

        public static ProjectVersion GetVersion(string projectFile)
        {
            var document = XDocument.Load(projectFile);
            var versionElement = document.XPathSelectElement(VersionXPath);

#pragma warning disable SA1000 // Keywords must be spaced correctly
            return versionElement is null ? default : new ProjectVersion(versionElement.Value);
#pragma warning restore SA1000 // Keywords must be spaced correctly
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