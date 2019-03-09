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

using McMaster.Extensions.CommandLineUtils;

namespace Ubiety.VersionIt.Commands
{
    /// <summary>
    ///     Base command class.
    /// </summary>
    [HelpOption]
    public abstract class VersionCommandBase
    {
        /// <summary>
        ///     Execute command.
        /// </summary>
        /// <param name="app">Application to use for command.</param>
        /// <returns>exit code.</returns>
        protected virtual int OnExecute(CommandLineApplication app)
        {
            return 1;
        }
    }
}
