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
    ///     Pre-release data for semantic version.
    /// </summary>
    public class PreReleaseTag : IFormattable, IEquatable<PreReleaseTag>, IComparable<PreReleaseTag>
    {
        private readonly EqualityHelper<PreReleaseTag> equality = new EqualityHelper<PreReleaseTag>(p => p.Name, p => p.Number);

        /// <summary>
        ///     Initializes a new instance of the <see cref="PreReleaseTag"/> class.
        /// </summary>
        public PreReleaseTag()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="PreReleaseTag"/> class.
        /// </summary>
        /// <param name="name">Pre-release name.</param>
        /// <param name="number">Pre-release build number.</param>
        public PreReleaseTag(string name, int? number)
        {
            Name = name;
            Number = number;
        }

        /// <summary>
        ///     Gets the pre-release name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     Gets the pre-release build number.
        /// </summary>
        public int? Number { get; }

        public static bool operator ==(PreReleaseTag left, PreReleaseTag right)
        {
            if (left is null)
            {
                return right is null;
            }

            return Equals(left, right);
        }

        public static bool operator !=(PreReleaseTag left, PreReleaseTag right)
        {
            return !(left == right);
        }

        public static bool operator >(PreReleaseTag left, PreReleaseTag right)
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

        public static bool operator <(PreReleaseTag left, PreReleaseTag right)
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

        public static bool operator >=(PreReleaseTag left, PreReleaseTag right)
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

        public static bool operator <=(PreReleaseTag left, PreReleaseTag right)
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

        /// <summary>
        ///     Parses a string into a new <see cref="PreReleaseTag"/> instance.
        /// </summary>
        /// <param name="tag">String of the version tag.</param>
        /// <returns>A new <see cref="PreReleaseTag"/> instance.</returns>
        public static PreReleaseTag Parse(string tag)
        {
            return default;
        }

        /// <inheritdoc />
        public int CompareTo(PreReleaseTag other)
        {
            if (!HasTag() && other.HasTag())
            {
                return 1;
            }

            if (HasTag() && !other.HasTag())
            {
                return -1;
            }

            var result = StringComparer.InvariantCultureIgnoreCase.Compare(Name, other.Name);
            if (result != 0)
            {
                return result;
            }

            return Nullable.Compare(Number, other.Number);
        }

        /// <inheritdoc />
        public bool Equals(PreReleaseTag other)
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

            return Equals(obj as PreReleaseTag);
        }

        /// <inheritdoc />
        public string ToString(string format, IFormatProvider formatProvider)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return equality.GetHashCode(this);
        }

        /// <summary>
        ///     Determines whether the instance has a pre-release tag.
        /// </summary>
        /// <returns>true if the instance has a tag; otherwise, false.</returns>
        public bool HasTag()
        {
            return !string.IsNullOrEmpty(Name);
        }
    }
}
