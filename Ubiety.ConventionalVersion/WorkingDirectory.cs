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

        public static WorkingDirectory DiscoverRepository(string directory)
        {
            var candidate = new DirectoryInfo(directory);

            if(!candidate.Exists)
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

            Exit($"Directory {directory} or parent is not a git repository", 3);

            return default;
        }

        public void UpdateVersion(bool skipDirtyCheck = false)
        {
            var workingDirectory = _workingDirectory.FullName;

            Information($"Working directory is {workingDirectory}");

            using(var repository = new Repository(workingDirectory))
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
            }
        }
    }
}
