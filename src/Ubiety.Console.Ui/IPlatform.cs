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
    ///     Output verbosity level.
    /// </summary>
    public enum VerbosityLevel
    {
        /// <summary>
        ///     Output all messages.
        /// </summary>
        All,

        /// <summary>
        ///     Silent
        /// </summary>
        Silent,
    }

    /// <summary>
    ///     Platform interface.
    /// </summary>
    public interface IPlatform
    {
        /// <summary>
        ///     Gets or sets the platform verbosity level.
        /// </summary>
        VerbosityLevel Verbosity { get; set; }

        /// <summary>
        ///     Exits the application.
        /// </summary>
        /// <param name="exitCode">Exit code.</param>
        void Exit(int exitCode);

        /// <summary>
        ///     Write a line to the console.
        /// </summary>
        /// <param name="message">Message to write.</param>
        /// <param name="color">Color to use.</param>
        /// <param name="formatters">Formatters to use on the message.</param>
        void WriteLine(string message, Color color, Formatter[] formatters = null);
    }
}
