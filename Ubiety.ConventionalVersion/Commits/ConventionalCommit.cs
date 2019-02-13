using System.Collections.Generic;
using Ubiety.VersionIt.Commits.Rules;

namespace Ubiety.ConventionalVersion.Commits
{
    public class ConventionalCommit
    {
        public string Scope { get; set; }
        public ConventionalTypes Type { get; set; }
        public string Subject { get; set; }
        public List<CommitNote> Notes { get; set; } = new List<CommitNote>();
    }
}