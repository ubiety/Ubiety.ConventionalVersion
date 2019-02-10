using System;
using System.Collections.Generic;
using System.Linq;
using Ubiety.ConventionalVersion.Commits;

namespace Ubiety.ConventionalVersion
{
    public enum VersionImpact
    {
        None,
        Patch,
        Minor,
        Major
    }

    public class VersionIncrementStrategy
    {
        private readonly VersionImpact _impact;

        private VersionIncrementStrategy(VersionImpact impact)
        {
            _impact = impact;
        }

        public Version NextVersion(Version version)
        {
            switch (_impact)
            {
                case VersionImpact.None:
                    return new Version(version.Major, version.Minor, version.Build + 1);
                case VersionImpact.Patch:
                    return new Version(version.Major, version.Minor, version.Build + 1);
                case VersionImpact.Minor:
                    return new Version(version.Major, version.Minor + 1, 0);
                case VersionImpact.Major:
                    return new Version(version.Major + 1, 0, 0);
                default:
                    return default;
            }
        }

        public static VersionIncrementStrategy Create(IEnumerable<ConventionalCommit> commits)
        {
            var impact = VersionImpact.None;

            foreach (var commit in commits)
            {
                if (!string.IsNullOrEmpty(commit.Type))
                {
                    switch (commit.Type)
                    {
                        case "feat":
                            impact = MaxImpact(impact, VersionImpact.Minor);
                            break;
                        case "fix":
                            impact = MaxImpact(impact, VersionImpact.Patch);
                            break;
                        default:
                            break;
                    }
                }

                if (commit.Notes.Any(note => note.Title.Equals("BREAKING CHANGE", StringComparison.InvariantCultureIgnoreCase)))
                {
                    impact = MaxImpact(impact, VersionImpact.Major);
                }
            }

            return new VersionIncrementStrategy(impact);
        }

        private static VersionImpact MaxImpact(VersionImpact left, VersionImpact right)
        {
            return (VersionImpact)Math.Max((int)left, (int)right);
        }
    }
}
