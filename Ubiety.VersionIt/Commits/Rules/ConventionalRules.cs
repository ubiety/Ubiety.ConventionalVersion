using System.Collections.Generic;

namespace Ubiety.VersionIt.Commits.Rules
{
    public enum ConventionalTypes
    {
        Fix,
        Feat,
        Docs,
        Ci,
        Build,
        Chore,
        Perf,
        Refactor,
        Revert,
        Style,
        Test
    }

    public static class ConventionalRules
    {
        public static Dictionary<ConventionalTypes, (string Type, string Header)> Rules => new Dictionary<ConventionalTypes, (string, string)>
        {
            { ConventionalTypes.Feat, ("feat", "Features") },
            { ConventionalTypes.Fix, ("fix", "Bug Fixes") },
            { ConventionalTypes.Chore, ("chore", "Chores") },
            { ConventionalTypes.Ci, ("ci", "Continuous Integration") },
            { ConventionalTypes.Docs, ("docs", "Documentation") },
            { ConventionalTypes.Build, ("build", "Build") },
            { ConventionalTypes.Perf, ("perf", "Performance") },
            { ConventionalTypes.Style, ("style", "Style") },
            { ConventionalTypes.Test, ("test", "Tests") },
            { ConventionalTypes.Revert, ("revert", "Reversions") },
            { ConventionalTypes.Refactor, ("refactor", "Refactors") }
        };
    }
}
