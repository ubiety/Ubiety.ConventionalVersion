using System;
using System.IO;
using System.Linq;
using LibGit2Sharp;
using static Ubiety.Console.Ui.CommandLine;

namespace Ubiety.ConventionalVersion
{
    public class WorkingDirectory
    {
        private readonly DirectoryInfo _workingDirectory;

        private WorkingDirectory(DirectoryInfo directory)
        {
            _workingDirectory = directory;
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
                    return new WorkingDirectory(candidate);
                }

                candidate = candidate.Parent;
            } while (candidate != null);

            Exit($"Directory {projectPath} or parent is not a git repository", 3);

            return default;
        }

        public WorkingDirectory UpdateVersion(bool skipDirtyCheck, string releaseAs, bool dryRun)
        {
            var workingDirectory = _workingDirectory.FullName;

            if (dryRun)
            {
                Information("DRY RUN - No changes will be committed");
            }

            Information($"Working directory is {workingDirectory}");

            using (var repository = new Repository(workingDirectory))
            {
                if (repository.RetrieveStatus().IsDirty && !skipDirtyCheck)
                {
                    Exit("Repository is dirty. Please commit your changes and try again", 1);
                }

                var projects = Project.DiscoverProjects(workingDirectory);

                if (projects.Count() == 0)
                {
                    Exit($"Could not find any projects in {workingDirectory} that have <Version> set.", 1);
                }

                Information($"Discovered {projects.Count()} versionable project(s)");
                foreach (var project in projects)
                {
                    Information($"  * {project.File}");
                }

                var nextVersion = projects.First().GetNextVersion(repository);

                if (!string.IsNullOrEmpty(releaseAs))
                {
                    nextVersion = new ProjectVersion(releaseAs);
                }

                foreach (var project in projects)
                {
                    if (nextVersion != projects.First().Version)
                    {
                        if (!dryRun)
                        {
                            project.SetVersion(nextVersion);
                            Commands.Stage(repository, project.File);
                        }
                        Step($"Bumping version from {project.Version} to {nextVersion} in project {project.File}");
                    }
                    else
                    {
                        Information($"No version change for project {project.File}");
                    }
                }
            }

            return this;
        }

        public void UpdateChangelog(bool dryRun)
        {

        }
    }
}
