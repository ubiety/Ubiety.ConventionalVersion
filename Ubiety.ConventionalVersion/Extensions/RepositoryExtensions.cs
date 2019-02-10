using System.Collections.Generic;
using System.Linq;
using LibGit2Sharp;

namespace Ubiety.ConventionalVersion.Extensions
{
    public static class RepositoryExtensions
    {
        public static Tag GetVersionTag(this Repository repository, ProjectVersion version)
        {
            return repository.Tags.SingleOrDefault(tag => tag.IsAnnotated && tag.Annotation.Name == $"v{version}");
        }

        public static IEnumerable<Commit> GetCommitsSinceLastVersion(this Repository repository, Tag tag)
        {
            if (tag is null)
            {
                return repository.Commits.ToList();
            }

            var filter = new CommitFilter
            {
                ExcludeReachableFrom = tag
            };

            return repository.Commits.QueryBy(filter).ToList();
        }
    }
}
