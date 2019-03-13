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

using System.Reflection;
using McMaster.Extensions.CommandLineUtils;
using Ubiety.Console;
using Ubiety.VersionIt.Commands;

namespace Ubiety.VersionIt
{
    /// <summary>
    ///     Main program class.
    /// </summary>
    [Command(Name = "versionit", Description = "Version your project based on your commits")]
    [VersionOptionFromMember(MemberName = nameof(Version))]
    [Subcommand(typeof(DotNetCommand), typeof(NpmCommand))]
    public class Program : VersionCommandBase
    {
        /// <summary>
        ///     Gets or sets a value indicating whether this is a dry run.
        /// </summary>
        [Option(Description = "Preview changes without updating files")]
        public bool DryRun { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the dirty check should be skipped.
        /// </summary>
        [Option("--skip-dirty", Description = "Skip dirty repository check")]
        public bool SkipDirty { get; set; }

        /// <summary>
        ///     Gets or sets the version to release the project as.
        /// </summary>
        [Option(Description = "Release as the manual version")]
        public string ReleaseAs { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether to run silently.
        /// </summary>
        [Option("--silent", Description = "Disable console output")]
        public bool Silent { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether to skip commiting the changes.
        /// </summary>
        [Option("--skip-commit", Description = "Skip committing changes after update")]
        public bool SkipCommit { get; set; }

        /// <summary>
        ///     Gets or sets the configuration file path.
        /// </summary>
        [Option(Description = "Configuration file path")]
        [FileExists]
        public string Config { get; set; }

        /// <summary>
        ///     Gets or sets the output type.
        /// </summary>
        [Option(Description = "Output type")]
        public OutputType Output { get; set; }

        /// <summary>
        ///     Gets or sets a value for the project path.
        /// </summary>
        [Argument(0, Description = "Git project directory or project file, will use current directory if not supplied")]
        [DirectoryExists]
        public string ProjectPath { get; set; }

        private string Version { get; } = typeof(Program).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;

        /// <inheritdoc />
        protected override int OnExecute(CommandLineApplication app)
        {
            CommandLine.Platform.Verbosity = Silent ? VerbosityLevel.Silent : VerbosityLevel.All;

            WorkingDirectory
                .DiscoverRepository(ProjectPath)
                .LoadConfiguration(Config)
                .UpdateVersion(SkipDirty, ReleaseAs, DryRun)
                .UpdateChangelog(DryRun)
                .CommitChanges(SkipCommit);

            return 0;
        }

        private static int Main(string[] args)
        {
            return CommandLineApplication.Execute<Program>(args);
        }
    }
}
