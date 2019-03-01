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

namespace Ubiety.VersionIt.Commits
{
    /// <summary>
    ///     Git commit note.
    /// </summary>
    public class CommitNote
    {
        /// <summary>
        ///     Gets or sets the note title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///     Gets or sets the note text.
        /// </summary>
        public string Text { get; set; }
    }
}