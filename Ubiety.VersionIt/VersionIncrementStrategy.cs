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
using System.Collections.Generic;
using System.Linq;
using Ubiety.ConventionalVersion.Commits;
using Ubiety.VersionIt.Commits.Rules;

namespace Ubiety.ConventionalVersion
{
    /// <summary>
    ///     Version impact.
    /// </summary>
    public enum VersionImpact
    {
        /// <summary>
        ///     No version impact.
        /// </summary>
        None,

        /// <summary>
        ///     Patch level version impact.
        /// </summary>
        Patch,

        /// <summary>
        ///     Minor level version impact.
        /// </summary>
        Minor,

        /// <summary>
        ///     Major level version impact.
        /// </summary>
        Major,
    }

    /// <summary>
    ///     Version incrementing strategy
    /// </summary>
    public class VersionIncrementStrategy
    {
        private readonly VersionImpact _impact;
        private readonly bool _isMaster;

        private VersionIncrementStrategy(VersionImpact impact, bool isMaster)
        {
            _impact = impact;
            _isMaster = isMaster;
        }

        /// <summary>
        ///     Create a new <see cref="VersionIncrementStrategy"/> instance.
        /// </summary>
        /// <param name="commits">List of commits.</param>
        /// <param name="isMaster">A value indicating whether the current branch is the master.</param>
        /// <returns>A <see cref="VersionIncrementStrategy"/> instance.</returns>
        public static VersionIncrementStrategy Create(IEnumerable<ConventionalCommit> commits, bool isMaster)
        {
            var impact = VersionImpact.None;

            foreach (var commit in commits)
            {
                switch (commit.Type)
                {
                    case ConventionalTypes.Feat:
                        impact = MaxImpact(impact, VersionImpact.Minor);
                        break;
                    case ConventionalTypes.Fix:
                        impact = MaxImpact(impact, VersionImpact.Patch);
                        break;
                }

                if (commit.Notes.Any(note =>
                    note.Title.Equals("BREAKING CHANGE", StringComparison.InvariantCultureIgnoreCase)))
                {
                    impact = MaxImpact(impact, VersionImpact.Major);
                }
            }

            return new VersionIncrementStrategy(impact, isMaster);
        }

        /// <summary>
        ///     Gets the next <see cref="ProjectVersion"/>.
        /// </summary>
        /// <param name="version">Previous <see cref="ProjectVersion"/>.</param>
        /// <returns>New <see cref="ProjectVersion"/>.</returns>
        public ProjectVersion NextVersion(ProjectVersion version)
        {
            switch (_impact)
            {
                case VersionImpact.None:
                    return version.ChangeSuffix(_isMaster);
                case VersionImpact.Patch:
                    return version.IncrementBuild(_isMaster);
                case VersionImpact.Minor:
                    return version.IncrementMinor(_isMaster);
                case VersionImpact.Major:
                    return version.IncrementMajor(_isMaster);
                default:
                    return default;
            }
        }

        private static VersionImpact MaxImpact(VersionImpact left, VersionImpact right)
        {
            return (VersionImpact)Math.Max((int)left, (int)right);
        }
    }
}