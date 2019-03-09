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

using System.Collections.Generic;

namespace Ubiety.VersionIt.Core.BuildServer
{
    /// <summary>
    ///     Base class for build servers.
    /// </summary>
    public class BuildServerBase : IBuildServer
    {
        private static readonly List<IBuildServer> BuildServers = new List<IBuildServer>
        {
            new AppVeyor(),
        };

        /// <summary>
        ///     Gets a value indicating whether the build server is active.
        /// </summary>
        public virtual bool Active { get; } = false;

        /// <summary>
        ///     Gets the current build server.
        /// </summary>
        /// <returns>Current active build server.</returns>
        public static IBuildServer GetBuildServer()
        {
            foreach (var buildServer in BuildServers)
            {
                if (buildServer.Active)
                {
                    return buildServer;
                }
            }

            return new BuildServerBase();
        }
    }
}
