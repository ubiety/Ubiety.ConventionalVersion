using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using LibGit2Sharp;
using Ubiety.VersionIt.Commits.Rules;

namespace Ubiety.ConventionalVersion.Commits
{
    public static class CommitParser
    {
        private static readonly Regex HeaderPattern =
            new Regex(
                "^(?<type>\\w*)(?:\\((?<scope>.*)\\))?: (?<subject>.*)$",
                RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Singleline);

        private static readonly string[] NoteKeywords = { "BREAKING CHANGE" };

        public static ConventionalCommit Parse(Commit commit)
        {
            var conventionalCommit = new ConventionalCommit();

            var commitLines = commit
                .Message
                .Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None)
                .Select(line => line.Trim())
                .Where(line => !string.IsNullOrEmpty(line))
                .ToList();

            var header = commitLines.FirstOrDefault();

            if (string.IsNullOrEmpty(header))
            {
                return conventionalCommit;
            }

            var headerParts = HeaderPattern.Match(header);

            if (headerParts.Success)
            {
                conventionalCommit.Scope = headerParts.Groups["scope"].Value;
                conventionalCommit.Subject = headerParts.Groups["subject"].Value;

                conventionalCommit.Type = ConventionalRules
                    .Rules
                    .FirstOrDefault(rule => rule.Value.Type.Equals(
                        headerParts.Groups["type"].Value,
                        StringComparison.InvariantCulture)).Key;
            }
            else
            {
                conventionalCommit.Subject = header;
            }

            for (var i = 1; i < commitLines.Count(); i++)
            {
                foreach (var keyword in NoteKeywords)
                {
                    if (commitLines[i].StartsWith(keyword, StringComparison.InvariantCulture))
                    {
                        conventionalCommit.Notes.Add(new CommitNote
                        {
                            Title = keyword,
                            Text = commitLines[i].Substring($"{keyword}:".Length).TrimStart()
                        });
                    }
                }
            }

            return conventionalCommit;
        }

        public static IEnumerable<ConventionalCommit> Parse(IEnumerable<Commit> commits)
        {
            return commits.Select(Parse).ToList();
        }
    }
}