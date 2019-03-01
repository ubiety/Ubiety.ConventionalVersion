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
using Ubiety.VersionIt.Commits;
using Ubiety.VersionIt.Commits.Rules;
using Ubiety.VersionIt.Extensions;

namespace Ubiety.VersionIt
{
    /// <summary>
    ///     Project information.
    /// </summary>
    public class Project
    {
        private const string VersionXPath = ".//PropertyGroup/Version";

        private Project(string file, ProjectVersion version)
        {
            File = file;
            Version = version;
        }

        /// <summary>
        ///     Gets the project file.
        /// </summary>
        public string File { get; }

        /// <summary>
        ///     Gets or sets the project version.
        /// </summary>
        public ProjectVersion Version { get; set; }

        /// <summary>
        ///     Gets the project commits.
        /// </summary>
        public IEnumerable<ConventionalCommit> Commits { get; private set; }

        /// <summary>
        ///     Gets the breaking change commits.
        /// </summary>
        public IEnumerable<ConventionalCommit> BreakingCommits => Commits?.Where(commit =>
            commit.Notes.Any(note => note.Title.Equals("BREAKING CHANGE", StringComparison.InvariantCulture)));

        /// <summary>
        ///     Discover projects.
        /// </summary>
        /// <param name="directory">Directory to search.</param>
        /// <returns>List of projects found.</returns>
        public static IEnumerable<Project> DiscoverProjects(string directory)
        {
            return Directory
                .GetFiles(directory, "*.csproj", SearchOption.AllDirectories)
                .Where(IsVersionable)
                .Select(Create)
                .ToList();
        }

        /// <summary>
        ///     Is the project versionable?.
        /// </summary>
        /// <param name="projectFile">Project file to check.</param>
        /// <returns>A value indicating whether the project can be versioned.</returns>
        public static bool IsVersionable(string projectFile)
        {
            var version = GetVersion(projectFile);
            return !(version is null);
        }

        /// <summary>
        ///     Gets the project version.
        /// </summary>
        /// <param name="projectFile">Project file to get the version for.</param>
        /// <returns>A <see cref="ProjectVersion"/> for the project.</returns>
        public static ProjectVersion GetVersion(string projectFile)
        {
            var document = XDocument.Load(projectFile);
            var versionElement = document.XPathSelectElement(VersionXPath);

            return versionElement is null ? default : new ProjectVersion(versionElement.Value);
        }

        /// <summary>
        ///     Create a new project instance.
        /// </summary>
        /// <param name="projectFile">Project to use for the project instance.</param>
        /// <returns>A <see cref="Project"/>.</returns>
        public static Project Create(string projectFile)
        {
            return new Project(projectFile, GetVersion(projectFile));
        }

        /// <summary>
        ///     Get commits for a specific commit type.
        /// </summary>
        /// <param name="type">Type of commit to get.</param>
        /// <returns>List of commits that match.</returns>
        public IEnumerable<ConventionalCommit> GetCommits(ConventionalTypes type)
        {
            return Commits.Where(commit => commit.Type == type);
        }

        /// <summary>
        ///     Get the next applicable version of the project.
        /// </summary>
        /// <param name="repository">Repository of the project.</param>
        /// <returns>A <see cref="ProjectVersion"/> representing the next version.</returns>
        public ProjectVersion GetNextVersion(Repository repository)
        {
            var versionTag = repository.GetVersionTag(Version);
            var commits = repository.GetCommitsSinceLastVersion(versionTag);

            var isMaster = repository.Head.FriendlyName == "master";

            Commits = CommitParser.Parse(commits);
            var incrementStrategy = VersionIncrementStrategy.Create(Commits, isMaster);

            return incrementStrategy.NextVersion(Version);
        }

        /// <summary>
        ///     Sets the version in the project file.
        /// </summary>
        /// <param name="nextVersion"><see cref="ProjectVersion"/> to set the project to.</param>
        public void SetVersion(ProjectVersion nextVersion)
        {
            var document = XDocument.Load(File);
            var versionElement = document.XPathSelectElement(VersionXPath);
            versionElement.Value = nextVersion;
            document.Save(File);
        }
    }
}
