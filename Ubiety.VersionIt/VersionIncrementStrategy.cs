using System;
using System.Collections.Generic;
using System.Linq;
using Ubiety.ConventionalVersion.Commits;
using Ubiety.VersionIt.Commits.Rules;

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
        private readonly bool _isMaster;

        private VersionIncrementStrategy(VersionImpact impact, bool isMaster)
        {
            _impact = impact;
            _isMaster = isMaster;
        }

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