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
    public class ProjectVersion
    {
        private readonly bool _isPreview;

        public ProjectVersion(Version version, bool isPreview = false, string previousTag = "")
        {
            Version = version;
            _isPreview = isPreview;
            PreviousTag = previousTag;
        }

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

        public Version Version { get; }

        public string Tag => $"v{ToString()}";

        public string PreviousTag { get; }

        public static implicit operator string(ProjectVersion version)
        {
            return version.ToString();
        }

        public static bool operator ==(ProjectVersion left, ProjectVersion right)
        {
            if (left is null)
            {
                return right is null;
            }

            return left.Equals(right);
        }

        public static bool operator !=(ProjectVersion left, ProjectVersion right)
        {
            if (left is null)
            {
                return right is null;
            }

            return !left.Equals(right);
        }

        public ProjectVersion IncrementBuild(bool isMaster)
        {
            return new ProjectVersion(new Version(Version.Major, Version.Minor, Version.Build + 1), !isMaster, Tag);
        }

        public ProjectVersion IncrementMinor(bool isMaster)
        {
            return new ProjectVersion(new Version(Version.Major, Version.Minor + 1, 0), !isMaster, Tag);
        }

        public ProjectVersion IncrementMajor(bool isMaster)
        {
            return new ProjectVersion(new Version(Version.Major + 1, 0, 0), !isMaster, Tag);
        }

        public ProjectVersion ChangeSuffix(bool isMaster)
        {
            return !_isPreview ? new ProjectVersion(Version, _isPreview) : new ProjectVersion(Version, !isMaster);
        }

        public override string ToString()
        {
            return $"{Version}{(_isPreview ? "-preview" : string.Empty)}";
        }

        public override bool Equals(object obj)
        {
#pragma warning disable SA1119 // Statement must not use unnecessary parenthesis
            return !(obj is null) && Version.Equals(((ProjectVersion)obj).Version);
#pragma warning restore SA1119 // Statement must not use unnecessary parenthesis
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}