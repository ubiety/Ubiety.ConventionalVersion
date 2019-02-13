using System.Collections.Generic;

namespace Ubiety.VersionIt.Commits.Rules
{
    public enum ConventionalTypes
    {
        fix,
        feat,
        docs,
        ci,
        build,
        chore,
        perf,
        refactor,
        revert,
        style,
        test
    }

    public static class ConventionalRules
    {
        public static Dictionary<ConventionalTypes, (string Type, string Header)> Rules => new Dictionary<ConventionalTypes, (string, string)>
        {
            { ConventionalTypes.feat, ("feat", "Features") },
            { ConventionalTypes.fix, ("fix", "Bug Fixes") },
            { ConventionalTypes.chore, ("chore", "Chores") },
            { ConventionalTypes.ci, ("ci", "Continuous Integration") },
            { ConventionalTypes.docs, ("docs", "Documentation") },
            { ConventionalTypes.build, ("build", "Build") },
            { ConventionalTypes.perf, ("perf", "Performance") },
            { ConventionalTypes.style, ("style", "Style") },
            { ConventionalTypes.test, ("test", "Tests") },
            { ConventionalTypes.revert, ("revert", "Reversions") },
            { ConventionalTypes.refactor, ("refactor", "Refactors") }
        };
    }
}
