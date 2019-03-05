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
using System.Text.RegularExpressions;
using Ubiety.VersionIt.Core.Exceptions;
using Ubiety.VersionIt.Core.Helpers;

namespace Ubiety.VersionIt.Core.Version
{
    /// <summary>
    ///     Semantic version representation.
    /// </summary>
    public sealed class SemanticVersion : IFormattable, IEquatable<SemanticVersion>, IComparable<SemanticVersion>
    {
        private static readonly Regex SemanticRegex =
            new Regex(RegexHelper.SemanticVersionRegex, RegexOptions.Compiled);

        private readonly EqualityHelper<SemanticVersion> _equality =
            new EqualityHelper<SemanticVersion>(s => s.Major, s => s.Minor, s => s.Patch, s => s.PreRelease);

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
        public int Major { get; private set; }

        /// <summary>
        ///     Gets the minor version number.
        /// </summary>
        public int Minor { get; private set; }

        /// <summary>
        ///     Gets the patch version number.
        /// </summary>
        public int Patch { get; private set; }

        /// <summary>
        ///     Gets the prerelease data.
        /// </summary>
        public PreReleaseTag PreRelease { get; private set; }

        public static implicit operator string(SemanticVersion semanticVersion)
        {
            return semanticVersion.ToString();
        }

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
                return false;
            }

            return left.CompareTo(right) > 0;
        }

        public static bool operator <(SemanticVersion left, SemanticVersion right)
        {
            if (left is null)
            {
                return true;
            }

            return left.CompareTo(right) < 0;
        }

        public static bool operator >=(SemanticVersion left, SemanticVersion right)
        {
            if (left is null)
            {
                return false;
            }

            return left.CompareTo(right) >= 0;
        }

        public static bool operator <=(SemanticVersion left, SemanticVersion right)
        {
            if (left is null)
            {
                return true;
            }

            return left.CompareTo(right) <= 0;
        }

        /// <summary>
        ///     Determines whether the provided instance is empty.
        /// </summary>
        /// <param name="version">Version instance to compare.</param>
        /// <returns>true if the version is empty; otherwise, false.</returns>
        public static bool IsEmpty(SemanticVersion version)
        {
            return Empty.Equals(version);
        }

        /// <summary>
        ///     Parses a string into a semantic version instance.
        /// </summary>
        /// <param name="version">Version to parse.</param>
        /// <param name="tagPrefix">Git tag version prefix.</param>
        /// <returns>A new <see cref="SemanticVersion"/> instance.</returns>
        public static SemanticVersion Parse(string version, string tagPrefix)
        {
            if (!TryParse(version, tagPrefix, out var semanticVersion))
            {
                throw new ParseException();
            }

            return semanticVersion;
        }

        /// <summary>
        ///     Parses a string into a semantic version instance.
        /// </summary>
        /// <param name="version">Version to parse.</param>
        /// <param name="tagPrefix">Git tag version prefix.</param>
        /// <param name="semanticVersion">Semantic version instance.</param>
        /// <returns>true if the parse was successful; otherwise, false.</returns>
        public static bool TryParse(string version, string tagPrefix, out SemanticVersion semanticVersion)
        {
            var match = Regex.Match(version, $@"^({tagPrefix})?(?<version>.*)$");

            if (!match.Success)
            {
                semanticVersion = default;
                return false;
            }

            var parsedVersion = SemanticRegex.Match(match.Groups["version"].Value);

            if (!parsedVersion.Success)
            {
                semanticVersion = default;
                return false;
            }

            semanticVersion = new SemanticVersion
            {
                Major = int.Parse(parsedVersion.Groups["Major"].Value, null),
                Minor = parsedVersion.Groups["Minor"].Success ? int.Parse(parsedVersion.Groups["Minor"].Value, null) : 0,
                Patch = parsedVersion.Groups["Patch"].Success ? int.Parse(parsedVersion.Groups["Patch"].Value, null) : 0,
                PreRelease = PreReleaseTag.Parse(parsedVersion.Groups["Tag"].Value),
            };

            return true;
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
            return _equality.Equals(this, other);
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
            return ToString("S", null);
        }

        /// <inheritdoc />
        public string ToString(string format, IFormatProvider formatProvider)
        {
            while (true)
            {
                if (string.IsNullOrEmpty(format))
                {
                    format = "J";
                }

                if (formatProvider?.GetFormat(GetType()) is ICustomFormatter formatter)
                {
                    return formatter.Format(format, this, formatProvider);
                }

                switch (format.ToUpperInvariant())
                {
                    case "J":
                        return $"{Major}.{Minor}.{Patch}";
                    case "S":
                        if (PreRelease.HasTag())
                        {
                            return $"{ToString("J", null)}-{PreRelease}";
                        }

                        format = "J";
                        formatProvider = null;
                        continue;
                    case "T":
                        if (PreRelease.HasTag())
                        {
                            return $"{ToString("J", null)}-{PreRelease.ToString("T", null)}";
                        }

                        format = "J";
                        formatProvider = null;
                        continue;
                    default:
                        throw new FormatException($"The '{format}' format string is not supported.");
                }
            }
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return _equality.GetHashCode(this);
        }
    }
}
