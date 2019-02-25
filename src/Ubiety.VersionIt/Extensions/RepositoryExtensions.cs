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
using System.Linq;
using LibGit2Sharp;

namespace Ubiety.ConventionalVersion.Extensions
{
    /// <summary>
    ///     Git repository extension methods.
    /// </summary>
    public static class RepositoryExtensions
    {
        /// <summary>
        ///     Get the version tag.
        /// </summary>
        /// <param name="repository">Repository to search.</param>
        /// <param name="version">Project version to look for.</param>
        /// <returns>Git tag for the version.</returns>
        public static Tag GetVersionTag(this Repository repository, ProjectVersion version)
        {
            return repository.Tags.SingleOrDefault(tag => tag.IsAnnotated && tag.Annotation.Name == version.Tag);
        }

        /// <summary>
        ///     Gets the commits since the last version tag.
        /// </summary>
        /// <param name="repository">Repository to search.</param>
        /// <param name="tag">Tag to search from.</param>
        /// <returns>List of commits.</returns>
        public static IEnumerable<Commit> GetCommitsSinceLastVersion(this Repository repository, Tag tag)
        {
            if (tag is null)
            {
                return repository.Commits.ToList();
            }

            var filter = new CommitFilter
            {
                ExcludeReachableFrom = tag,
            };

            return repository.Commits.QueryBy(filter).ToList();
        }
    }
}