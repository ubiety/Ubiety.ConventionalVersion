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

using System.Drawing;
using Colorful;

namespace Ubiety.Console
{
    /// <summary>
    ///     Command line helper methods.
    /// </summary>
    public static class CommandLine
    {
        /// <summary>
        ///     Gets the platform.
        /// </summary>
        public static IPlatform Platform { get; } = new Platform { Verbosity = VerbosityLevel.All };

        /// <summary>
        ///     Outputs a message and exits the application.
        /// </summary>
        /// <param name="message">Message to output.</param>
        /// <param name="exitCode">Exit code.</param>
        public static void Exit(string message, int exitCode)
        {
            Platform.WriteLine(message, Color.Red);
            Platform.Exit(exitCode);
        }

        /// <summary>
        ///     Outputs an informational message.
        /// </summary>
        /// <param name="message">Message to output.</param>
        public static void Information(string message)
        {
            Platform.WriteLine(message, Color.LightGray);
        }

        /// <summary>
        ///     Outputs a step message.
        /// </summary>
        /// <param name="message">Message to output.</param>
        public static void Step(string message)
        {
            var stepMessage = "{0} {1}";
            var stepFormatters = new[]
            {
                new Formatter("√", Color.Green),
                new Formatter(message, Color.LightGray),
            };

            Platform.WriteLine(stepMessage, Color.White, stepFormatters);
        }
    }
}
