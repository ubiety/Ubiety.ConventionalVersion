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

using System.Threading.Tasks;
using Ubiety.VersionIt.Core.BuildServer;

namespace Ubiety.VersionIt.Core
{
    /// <summary>
    ///     Main VersionIt class.
    /// </summary>
    public class VersionIt
    {
        private string _workingDirectory;

        /// <summary>
        ///     Initializes a new instance of the <see cref="VersionIt"/> class.
        /// </summary>
        /// <param name="workingDirectory">Working directory of the project.</param>
        public VersionIt(string workingDirectory)
        {
            _workingDirectory = workingDirectory;
        }

        /// <summary>
        ///     Run the version task.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<int> RunAsync()
        {
            var buildServer = BuildServerBase.GetBuildServer();

            return 0;
        }
    }
}
