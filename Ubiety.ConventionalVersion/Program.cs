using McMaster.Extensions.CommandLineUtils;
using Ubiety.Console.Ui;

namespace Ubiety.ConventionalVersion
{
    [Command(Name = "versionit", Description = "Version your dotnet app based on your commits")]
    [HelpOption]
    [VersionOptionFromMember(MemberName = "Version")]
    public class Program
    {
        [Option(Description = "Execute without actually committing")]
        public bool DryRun { get; set; }

        [Option("--skip-dirty", Description = "Skip dirty repository check")]
        public bool SkipDirty { get; set; }

        [Option(Description = "Release as the manual version")]
        public string ReleaseAs { get; set; }

        [Option("--silent", Description = "Disable console output")]
        public bool Silent { get; set; }

        [Option("--skip-commit", Description = "Skip committing changes after update")]
        public bool SkipCommit { get; set; }

        [Argument(0, Description = "Git project directory or csproj file, will use current directory if not supplied")]
        public string ProjectPath { get; set; }

        private string Version { get; } = typeof(Program).Assembly.GetName().Version.ToString();

        private static int Main(string[] args)
        {
            return CommandLineApplication.Execute<Program>(args);
        }

        private int OnExecute()
        {
            CommandLine.Platform.Verbosity = Silent ? VerbosityLevel.Silent : VerbosityLevel.All;

            WorkingDirectory
                .DiscoverRepository(ProjectPath)
                .UpdateVersion(SkipDirty, ReleaseAs, DryRun)
                .UpdateChangelog(DryRun)
                .CommitChanges(SkipCommit);

            return 0;
        }
    }
}