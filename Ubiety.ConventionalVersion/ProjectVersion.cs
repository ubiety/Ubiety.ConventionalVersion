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

        public static implicit operator string(ProjectVersion version)
        {
            return version.ToString();
        }

        public static bool operator ==(ProjectVersion left, ProjectVersion right)
        {
            if (left is null) return right is null;

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
            return !(obj is null) && Version.Equals(((ProjectVersion) obj).Version);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}