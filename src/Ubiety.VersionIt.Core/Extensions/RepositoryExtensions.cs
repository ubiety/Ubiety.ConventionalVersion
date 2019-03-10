/* Copyright 2019 Dieter Lunn
 *
 *   Licensed under the Apache License, Version 2.0 (the "License");
 *   you may not use this file except in compliance with the License.
 *   You may obtain a copy of the License at
 *
 *       http://www.apache.org/licenses/LICENSE-2.0
 *
 *   Unless required by applicable law or agreed to in writing, software
 *   distributed under the License is distributed on an "AS IS" BASIS,
 *   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *   See the License for the specific language governing permissions and
 *   limitations under the License.
 */

using System;
using System.Linq;
using LibGit2Sharp;

namespace Ubiety.VersionIt.Core.Extensions
{
    /// <summary>
    ///     Extensions for git repository.
    /// </summary>
    public static class RepositoryExtensions
    {
        /// <summary>
        ///     Normalize the git repository.
        /// </summary>
        /// <param name="repository"><see cref="IRepository"/> to normalize.</param>
        /// <param name="currentBranch">Current git repository branch.</param>
        public static void Normalize(this Repository repository, string currentBranch)
        {
            var expectedSha = repository.Head.Tip.Sha;
            Remote remote;

            try
            {
                remote = repository.Network.Remotes.Single();
            }
            catch (InvalidOperationException)
            {
                return;
            }

            repository.AddMissingRefSpecs(remote);

            Commands.Fetch(repository, remote.Name, Array.Empty<string>(), null, null);

            repository.EnsureLocalBranch(remote, currentBranch);
            repository.UpdateLocalBranchesFromRemote(remote.Name);

            var headSha = repository.Refs.Head.TargetIdentifier;

            if (!repository.Info.IsHeadDetached)
            {
                return;
            }

            var branchShas = repository.Branches.Where(branch => !branch.IsRemote && branch.Tip.Sha == headSha).ToList();
        }

        /// <summary>
        ///     Adds missing refspecs to git remote.
        /// </summary>
        /// <param name="repository">Repository to update.</param>
        /// <param name="remote">Remote to add specs to.</param>
        public static void AddMissingRefSpecs(this IRepository repository, Remote remote)
        {
            if (remote.FetchRefSpecs.Any(r => r.Source == "refs/heads/*"))
            {
                return;
            }

            repository.Network.Remotes.Update(remote.Name, r => r.FetchRefSpecs.Add($"+refs/heads/*:refs/remotes/{remote.Name}/*"));
        }

        /// <summary>
        ///     Ensures there is a local branch for the current remote branch.
        /// </summary>
        /// <param name="repository">Repository to update.</param>
        /// <param name="remote">Remote to compare.</param>
        /// <param name="currentBranch">Branch to checkout.</param>
        public static void EnsureLocalBranch(this IRepository repository, Remote remote, string currentBranch)
        {
            if (string.IsNullOrEmpty(currentBranch))
            {
                return;
            }

            var isRef = currentBranch.Contains("refs");
            var isBranch = currentBranch.Contains("refs/heads");
            var branchName = isBranch ? currentBranch : currentBranch.Replace("refs/", "refs/heads/");
            var localCanonicalName = isRef ? branchName : $"refs/heads/{currentBranch}";

            var tip = repository.Head.Tip;

            var originCanonicalName = $"{remote.Name}/{currentBranch}";
            var originBranch = repository.Branches[originCanonicalName];
            if (originBranch != null)
            {
                tip = originBranch.Tip;
            }

            if (repository.Branches.All(branch => branch.CanonicalName != localCanonicalName))
            {
                repository.Refs.Add(localCanonicalName, tip.Id);
            }
            else
            {
                repository.Refs.UpdateTarget(repository.Refs[localCanonicalName], tip.Id);
            }

            Commands.Checkout(repository, localCanonicalName);
        }

        /// <summary>
        ///     Updates local branches.
        /// </summary>
        /// <param name="repository">Repository to use.</param>
        /// <param name="remoteName">Remote to update from.</param>
        public static void UpdateLocalBranchesFromRemote(this IRepository repository, string remoteName)
        {
            var prefix = $"refs/remotes/{remoteName}/";
            var remoteHeadName = $"{prefix}HEAD";

            foreach (var remoteReference in repository.Refs.FromGlob($"{prefix}*").Where(r => r.CanonicalName != remoteHeadName))
            {
                var referenceName = remoteReference.CanonicalName;
                var branchName = referenceName.Substring(prefix.Length);

                if (branchName == repository.Head.FriendlyName)
                {
                    continue;
                }

                var localCanonicalName = $"refs/heads/{branchName}";

                if (repository.Refs.Any(r => r.CanonicalName == localCanonicalName))
                {
                    var localRef = repository.Refs[localCanonicalName];
                    var remoteRef = remoteReference.ResolveToDirectReference();

                    if (localRef.ResolveToDirectReference().TargetIdentifier == remoteRef.TargetIdentifier)
                    {
                        continue;
                    }

                    repository.Refs.UpdateTarget(localRef, remoteRef.Target.Id);
                    continue;
                }

                repository.Refs.Add(localCanonicalName, new ObjectId(remoteReference.ResolveToDirectReference().TargetIdentifier), true);
                var branch = repository.Branches[branchName];
                repository.Branches.Update(branch, b => b.TrackedBranch = referenceName);
            }
        }
    }
}
