using System;

namespace Ubiety.ConventionalVersion
{
    public class ProjectVersion
    {
        private readonly bool _isPreview;

        public ProjectVersion(Version version, bool isPreview = false)
        {
            Version = version;
            _isPreview = isPreview;
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
                Version = new Version(version.Substring(0, index - 1));
            }
        }

        public Version Version { get; set; }

        public ProjectVersion IncrementBuild(bool isMaster)
        {
            return new ProjectVersion(new Version(Version.Major, Version.Minor, Version.Build + 1), !isMaster);
        }

        public ProjectVersion IncrementMinor(bool isMaster)
        {
            return new ProjectVersion(new Version(Version.Major, Version.Minor + 1, 0), !isMaster);
        }

        public ProjectVersion IncrementMajor(bool isMaster)
        {
            return new ProjectVersion(new Version(Version.Major + 1, 0, 0), !isMaster);
        }

        public ProjectVersion ChangeSuffix(bool isMaster)
        {
            if (!_isPreview)
            {
                return new ProjectVersion(Version, _isPreview);
            }

            return new ProjectVersion(Version, !isMaster);
        }

        public static implicit operator string(ProjectVersion version)
        {
            return version.ToString();
        }

        public static implicit operator ProjectVersion(Version version)
        {
            return new ProjectVersion(version);
        }

        public static explicit operator Version(ProjectVersion version)
        {
            return version.Version;
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
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return $"{Version}{(_isPreview ? "-preview" : "")}";
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }

            return Version.Equals(((ProjectVersion)obj).Version);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
