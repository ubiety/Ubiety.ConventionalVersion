using System.Collections.Generic;

namespace Ubiety.ConventionalVersion.Commits
{
    public class ConventionalCommit
    {
        public string Scope { get; set; }
        public string Type { get; set; }
        public string Subject { get; set; }
        public List<CommitNote> Notes { get; set; } = new List<CommitNote>();
    }
}