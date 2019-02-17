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
using Ubiety.VersionIt.Commits.Rules;

namespace Ubiety.ConventionalVersion.Commits
{
    /// <summary>
    ///     Conventional commit.
    /// </summary>
    public class ConventionalCommit
    {
        /// <summary>
        ///     Gets or sets the commit scope.
        /// </summary>
        public string Scope { get; set; }

        /// <summary>
        ///     Gets or sets the commit type.
        /// </summary>
        public ConventionalTypes Type { get; set; }

        /// <summary>
        ///     Gets or sets the commit subject.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        ///     Gets or sets the commit notes.
        /// </summary>
        public List<CommitNote> Notes { get; set; } = new List<CommitNote>();
    }
}