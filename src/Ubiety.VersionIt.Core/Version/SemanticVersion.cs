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
using Ubiety.VersionIt.Core.Helpers;

namespace Ubiety.VersionIt.Core.Version
{
    /// <summary>
    ///     Semantic version representation.
    /// </summary>
    public class SemanticVersion : IFormattable, IEquatable<SemanticVersion>, IComparable<SemanticVersion>
    {
        private readonly EqualityHelper<SemanticVersion> equality = new EqualityHelper<SemanticVersion>(s => s.Major, s => s.Minor, s => s.Patch, s => s.PreRelease);

        /// <summary>
        ///     Initializes a new instance of the <see cref="SemanticVersion"/> class.
        /// </summary>
        /// <param name="major">Major version number.</param>
        /// <param name="minor">Minor version number.</param>
        /// <param name="patch">Patch version number.</param>
        public SemanticVersion(int major = 0, int minor = 0, int patch = 0)
        {
            Major = major;
            Minor = minor;
            Patch = patch;
        }

        /// <summary>
        ///     Gets an empty version.
        /// </summary>
        public static SemanticVersion Empty => new SemanticVersion();

        /// <summary>
        ///     Gets the major version number.
        /// </summary>
        public int Major { get; }

        /// <summary>
        ///     Gets the minor version number.
        /// </summary>
        public int Minor { get; }

        /// <summary>
        ///     Gets the patch version number.
        /// </summary>
        public int Patch { get; }

        /// <summary>
        ///     Gets the prerelease data.
        /// </summary>
        public PreReleaseTag PreRelease { get; }

        public static bool operator ==(SemanticVersion left, SemanticVersion right)
        {
            if (left is null)
            {
                return right is null;
            }

            return left.Equals(right);
        }

        public static bool operator !=(SemanticVersion left, SemanticVersion right)
        {
            return !(left == right);
        }

        public static bool operator >(SemanticVersion left, SemanticVersion right)
        {
            if (left is null)
            {
                throw new ArgumentNullException(nameof(left));
            }

            if (right is null)
            {
                throw new ArgumentNullException(nameof(right));
            }

            return left.CompareTo(right) > 0;
        }

        public static bool operator <(SemanticVersion left, SemanticVersion right)
        {
            if (left is null)
            {
                throw new ArgumentNullException(nameof(left));
            }

            if (right is null)
            {
                throw new ArgumentNullException(nameof(right));
            }

            return left.CompareTo(right) < 0;
        }

        public static bool operator >=(SemanticVersion left, SemanticVersion right)
        {
            if (left is null)
            {
                throw new ArgumentNullException(nameof(left));
            }

            if (right is null)
            {
                throw new ArgumentNullException(nameof(right));
            }

            return left.CompareTo(right) >= 0;
        }

        public static bool operator <=(SemanticVersion left, SemanticVersion right)
        {
            if (left is null)
            {
                throw new ArgumentNullException(nameof(left));
            }

            if (right is null)
            {
                throw new ArgumentNullException(nameof(right));
            }

            return left.CompareTo(right) <= 0;
        }

        /// <inheritdoc />
        public int CompareTo(SemanticVersion other)
        {
            if (other is null)
            {
                return 1;
            }

            if (Major != other.Major)
            {
                if (Major > other.Major)
                {
                    return 1;
                }

                return -1;
            }

            if (Minor != other.Minor)
            {
                if (Minor > other.Minor)
                {
                    return 1;
                }

                return -1;
            }

            if (Patch != other.Patch)
            {
                if (Patch > other.Patch)
                {
                    return 1;
                }

                return -1;
            }

            if (PreRelease != other.PreRelease)
            {
                if (PreRelease > other.PreRelease)
                {
                    return 1;
                }

                return -1;
            }

            return 0;
        }

        /// <inheritdoc />
        public bool Equals(SemanticVersion other)
        {
            return equality.Equals(this, other);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (GetType() != obj.GetType())
            {
                return false;
            }

            return Equals(obj as SemanticVersion);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return ToString(null);
        }

        /// <inheritdoc />
        public string ToString(string format, IFormatProvider formatProvider = null)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return equality.GetHashCode(this);
        }
    }
}
