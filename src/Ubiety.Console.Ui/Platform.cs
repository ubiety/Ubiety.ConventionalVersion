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
using System.Drawing;
using Colorful;

namespace Ubiety.Console
{
    /// <summary>
    ///     Platform implementation.
    /// </summary>
    public class Platform : IPlatform
    {
        /// <inheritdoc />
        public VerbosityLevel Verbosity { get; set; }

        /// <inheritdoc />
        public void Exit(int exitCode)
        {
            Environment.Exit(exitCode);
        }

        /// <inheritdoc />
        public void WriteLine(string message, Color color, Formatter[] formatters = null)
        {
            if (Verbosity == VerbosityLevel.Silent)
            {
                return;
            }

            if (formatters is null)
            {
                Colorful.Console.WriteLine(message, color);
            }
            else
            {
                Colorful.Console.WriteLineFormatted(message, color, formatters);
            }
        }

        /// <summary>
        ///     Write a line to the console.
        /// </summary>
        /// <param name="message">Message to write.</param>
        /// <param name="formatters">Formatters to use on the message.</param>
        public void WriteLine(string message, Formatter[] formatters = null)
        {
            WriteLine(message, Color.Empty, formatters);
        }
    }
}
