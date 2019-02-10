using System;

namespace Ubiety.ConventionalVersion
{
    public class ProjectVersion
    {
        private bool _isPreview;

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

        public string Tag { get => $"v{Version}"; }

        public string PreviousTag { get; private set; }

        public ProjectVersion IncrementBuild(bool isMaster)
        {
            PreviousTag = Tag;
            Version = new Version(Version.Major, Version.Minor, Version.Build + 1);
            _isPreview = !isMaster;

            return this;
        }

        public ProjectVersion IncrementMinor(bool isMaster)
        {
            PreviousTag = Tag;
            Version = new Version(Version.Major, Version.Minor + 1, 0);
            _isPreview = !isMaster;

            return this;
        }

        public ProjectVersion IncrementMajor(bool isMaster)
        {
            PreviousTag = Tag;
            Version = new Version(Version.Major + 1, 0, 0);
            _isPreview = !isMaster;

            return this;
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
