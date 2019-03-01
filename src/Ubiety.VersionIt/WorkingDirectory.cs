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
using LibGit2Sharp;
using static Ubiety.Console.Ui.CommandLine;

namespace Ubiety.ConventionalVersion
{
    /// <summary>
    ///     Working directory.
    /// </summary>
    public class WorkingDirectory
    {
        private readonly Repository _repository;
        private readonly string _workingDirectoryName;
        private IEnumerable<Project> _projects;

        private WorkingDirectory(string directoryName)
        {
            _workingDirectoryName = directoryName;
            _repository = new Repository(_workingDirectoryName);
        }

        /// <summary>
        ///     Discover the repository path.
        /// </summary>
        /// <param name="projectPath">Path of the current project.</param>
        /// <returns>A new <see cref="WorkingDirectory"/> instance.</returns>
        public static WorkingDirectory DiscoverRepository(string projectPath)
        {
            var directory = string.Empty;

            if (string.IsNullOrEmpty(projectPath))
            {
                directory = Directory.GetCurrentDirectory();
            }
            else
            {
                if (Path.HasExtension(projectPath))
                {
                    if (".csproj".Equals(Path.GetExtension(projectPath), StringComparison.InvariantCultureIgnoreCase))
                    {
                        directory = Path.GetDirectoryName(projectPath);
                    }
                }
                else
                {
                    directory = projectPath;
                }
            }

            var candidate = new DirectoryInfo(directory);

            if (!candidate.Exists)
            {
                Exit("Working directory does not exist", 2);
            }

            do
            {
                var isWorkingDirectory = candidate.GetDirectories(".git").Any();
                if (isWorkingDirectory)
                {
                    return new WorkingDirectory(candidate.FullName);
                }

                candidate = candidate.Parent;
            }
            while (candidate != null);

            Exit($"Directory {projectPath} or parent is not a git repository", 3);

            return default;
        }

        /// <summary>
        ///     Update the project version.
        /// </summary>
        /// <param name="skipDirtyCheck">A value indicating whether to skip the repository dirty check.</param>
        /// <param name="releaseAs">Version to release the project as.</param>
        /// <param name="dryRun">A value indicating whether this is a dry run.</param>
        /// <returns>A <see cref="WorkingDirectory"/> instance.</returns>
        public WorkingDirectory UpdateVersion(bool skipDirtyCheck, string releaseAs, bool dryRun)
        {
            if (dryRun)
            {
                Information("DRY RUN - No changes will be committed");
            }

            Information($"Working directory is {_workingDirectoryName}");

            if (_repository.RetrieveStatus().IsDirty && !skipDirtyCheck)
            {
                Exit("Repository is dirty. Please commit your changes and try again", 1);
            }

            _projects = Project.DiscoverProjects(_workingDirectoryName);

            if (!_projects.Any())
            {
                Exit($"Could not find any projects in {_workingDirectoryName} that have <Version> set.", 1);
            }

            Information($"Discovered {_projects.Count()} versionable project(s)");
            foreach (var project in _projects)
            {
                Information($"  * {project.File}");
            }

            var nextVersion = _projects.First().GetNextVersion(_repository);

            if (!string.IsNullOrEmpty(releaseAs))
            {
                nextVersion = new ProjectVersion(releaseAs);
            }

            foreach (var project in _projects)
            {
                if (nextVersion != project.Version)
                {
                    Step($"Bumping version from {project.Version} to {nextVersion} in project {project.File}");
                    project.Version = nextVersion;
                    if (!dryRun)
                    {
                        project.SetVersion(nextVersion);
                        Commands.Stage(_repository, project.File);
                    }
                }
                else
                {
                    Information($"No version change for project {project.File}");
                }
            }

            return this;
        }

        /// <summary>
        ///     Update the changelog.
        /// </summary>
        /// <param name="dryRun">A value indicating whether this is a dry run.</param>
        /// <returns>A <see cref="WorkingDirectory"/> instance.</returns>
        public WorkingDirectory UpdateChangelog(bool dryRun)
        {
            var changelog = Changelog.DiscoverChangelog(_workingDirectoryName);
            var changelogText = Changelog.UpdateChangelog(_projects.First(), _repository);

            if (dryRun)
            {
                Information(changelogText);
            }
            else
            {
                changelog.WriteFile(changelogText);
            }

            Commands.Stage(_repository, changelog.FilePath);
            Step("Updated CHANGELOG.md");

            return this;
        }

        /// <summary>
        ///     Commit changes to the repository.
        /// </summary>
        /// <param name="skipCommit">A value indicating whether to skip the commit.</param>
        public void CommitChanges(bool skipCommit)
        {
            if (skipCommit)
            {
                Information(
                    $"Commit and tagging of version was skipped. Tag release as `{_projects.First().Version.Tag}` so versionit can detect the next version.");
                return;
            }

            foreach (var project in _projects)
            {
                Commands.Stage(_repository, project.File);
            }

            var firstProject = _projects.First();

            var author = _repository.Config.BuildSignature(DateTimeOffset.Now);
            var commitMessage = $"chore(release): {firstProject.Version}";

            var versionCommit = _repository.Commit(commitMessage, author, author);
            Step("Committed changes to projects and CHANGELOG.md");
            _ = _repository.Tags.Add(
                firstProject.Version.Tag,
                versionCommit,
                author,
                $"{firstProject.Version.Version}");
            Step($"Tagged new version as {firstProject.Version.Tag}");

            Information(string.Empty);
            Information("Run `git push --follow-tags origin master` to push changes and tags to origin");
        }
    }
}
