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

namespace Ubiety.ConventionalVersion
{
    /// <summary>
    ///     Project version.
    /// </summary>
    public class ProjectVersion
    {
        private readonly bool _isPreview;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ProjectVersion"/> class.
        /// </summary>
        /// <param name="version"><see cref="Version"/> to use.</param>
        /// <param name="isPreview">A value indicating whether the project is a preview build.</param>
        /// <param name="previousTag">Previous git tag.</param>
        public ProjectVersion(Version version, bool isPreview = false, string previousTag = "")
        {
            Version = version;
            _isPreview = isPreview;
            PreviousTag = previousTag;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ProjectVersion"/> class.
        /// </summary>
        /// <param name="version">String implementation of the version.</param>
        public ProjectVersion(string version)
        {
            var index = version.IndexOf("-", StringComparison.InvariantCulture);
            if (index < 0)
            {
                Version = new Version(version);
            }
            else
            {
                _isPreview = true;
                Version = new Version(version.Substring(0, index));
            }
        }

        /// <summary>
        ///     Gets the <see cref="Version"/>.
        /// </summary>
        public Version Version { get; }

        /// <summary>
        ///     Gets the git tag for the version.
        /// </summary>
        public string Tag => $"v{ToString()}";

        /// <summary>
        ///     Gets the previous git tag.
        /// </summary>
        public string PreviousTag { get; }

        /// <summary>
        ///     Converts the <see cref="ProjectVersion"/> to a string.
        /// </summary>
        /// <param name="version"><see cref="ProjectVersion"/> to convert.</param>
        public static implicit operator string(ProjectVersion version)
        {
            return version.ToString();
        }

        /// <summary>
        ///     Are the two <see cref="ProjectVersion"/> classes equal.
        /// </summary>
        /// <param name="left">Left <see cref="ProjectVersion"/>.</param>
        /// <param name="right">Right <see cref="ProjectVersion"/>.</param>
        /// <returns>A value indicating whether the versions are equal.</returns>
        public static bool operator ==(ProjectVersion left, ProjectVersion right)
        {
            if (left is null)
            {
                return right is null;
            }

            return left.Equals(right);
        }

        /// <summary>
        ///     Are the two <see cref="ProjectVersion"/> classes not equal.
        /// </summary>
        /// <param name="left">Left <see cref="ProjectVersion"/>.</param>
        /// <param name="right">Right <see cref="ProjectVersion"/>.</param>
        /// <returns>A value indicating whether the versions are not equal.</returns>
        public static bool operator !=(ProjectVersion left, ProjectVersion right)
        {
            if (left is null)
            {
                return right is null;
            }

            return !left.Equals(right);
        }

        /// <summary>
        ///     Increment the build number.
        /// </summary>
        /// <param name="isMaster">Is the current branch the master.</param>
        /// <returns>A new <see cref="ProjectVersion"/> with the incremented version.</returns>
        public ProjectVersion IncrementBuild(bool isMaster)
        {
            return new ProjectVersion(new Version(Version.Major, Version.Minor, Version.Build + 1), !isMaster, Tag);
        }

        /// <summary>
        ///     Increment the minor number.
        /// </summary>
        /// <param name="isMaster">A value indicating whether the current branch is the master.</param>
        /// <returns>A new <see cref="ProjectVersion"/> instance with the incremented version.</returns>
        public ProjectVersion IncrementMinor(bool isMaster)
        {
            return new ProjectVersion(new Version(Version.Major, Version.Minor + 1, 0), !isMaster, Tag);
        }

        /// <summary>
        ///     Increment the major number.
        /// </summary>
        /// <param name="isMaster">A value indicating whether the current branch is the master.</param>
        /// <returns>A new <see cref="ProjectVersion"/> instance with the incremented version.</returns>
        public ProjectVersion IncrementMajor(bool isMaster)
        {
            return new ProjectVersion(new Version(Version.Major + 1, 0, 0), !isMaster, Tag);
        }

        /// <summary>
        ///     Change the version suffix.
        /// </summary>
        /// <param name="isMaster">A value indicating whether the current branch is the master.</param>
        /// <returns>A new <see cref="ProjectVersion"/> instance with the new suffix.</returns>
        public ProjectVersion ChangeSuffix(bool isMaster)
        {
            return !_isPreview ? new ProjectVersion(Version, _isPreview) : new ProjectVersion(Version, !isMaster);
        }

        /// <summary>
        ///     Converts the current instance to a string.
        /// </summary>
        /// <returns>A string representation of the version.</returns>
        public override string ToString()
        {
            return $"{Version}{(_isPreview ? "-preview" : string.Empty)}";
        }

        /// <summary>
        ///     Checks whether the two objects are equal.
        /// </summary>
        /// <param name="obj"><see cref="object"/> to compare the current instance to.</param>
        /// <returns>A value indicating whether the two objects are equal.</returns>
        public override bool Equals(object obj)
        {
            return !(obj is null) && Version.Equals(((ProjectVersion)obj).Version);
        }

        /// <summary>
        ///     Gets the hash code for the current instance.
        /// </summary>
        /// <returns>Object hash code</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}