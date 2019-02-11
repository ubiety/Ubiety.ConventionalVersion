using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LibGit2Sharp;
using static Ubiety.Console.Ui.CommandLine;

namespace Ubiety.ConventionalVersion
{
    public class WorkingDirectory
    {
        private readonly string _workingDirectoryName;
        private readonly Repository _repository;
        private IEnumerable<Project> _projects;

        private WorkingDirectory(string directoryName)
        {
            _workingDirectoryName = directoryName;
            _repository = new Repository(_workingDirectoryName);
        }

        public static WorkingDirectory DiscoverRepository(string projectPath)
        {
            var directory = "";

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
            } while (candidate != null);

            Exit($"Directory {projectPath} or parent is not a git repository", 3);

            return default;
        }

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

            if (_projects.Count() == 0)
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
                    if (!dryRun)
                    {
                        project.SetVersion(nextVersion);
                        Commands.Stage(_repository, project.File);
                    }
                    Step($"Bumping version from {project.Version} to {nextVersion} in project {project.File}");
                }
                else
                {
                    Information($"No version change for project {project.File}");
                }
            }

            Information("");

            Information($"VERSIONIT_NUGET_VERSION: {nextVersion}");
            Environment.SetEnvironmentVariable("VERSIONIT_NUGET_VERSION", nextVersion);

            Information($"VERSIONIT_CI_VERSION: {nextVersion.Version}");
            Environment.SetEnvironmentVariable("VERSIONIT_CI_VERSION", nextVersion.Version.ToString());

            Information("");

            return this;
        }

        public WorkingDirectory UpdateChangelog(bool dryRun)
        {
            var changelog = Changelog.DiscoverChangelog(_workingDirectoryName);

            if (dryRun)
            {
                Information(changelog.UpdateChangelog(_projects.First(), _repository));
            }

            return default;
        }
    }
}
