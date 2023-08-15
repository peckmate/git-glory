using System;
using System.IO;
using System.Linq;
using LibGit2Sharp;
using System.Collections.Generic;
using ConsoleTableExt;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Give me a path");
        var repositoryPath = Console.ReadLine();

        using (var repo = new Repository(repositoryPath))
        {
            var authorChanges = new Dictionary<string, (int additions, int deletions)>();

            foreach (var commit in repo.Commits)
            {
                var changes = commit.Parents.Any()
                    ? repo.Diff.Compare<Patch>(commit.Parents.First().Tree, commit.Tree)
                    : repo.Diff.Compare<Patch>(null, commit.Tree);

                string authorName = commit.Author.Name;

                int additions = changes.Sum(change => change.LinesAdded);
                int deletions = changes.Sum(change => change.LinesDeleted);

                if (authorChanges.ContainsKey(authorName))
                {
                    var (prevAdditions, prevDeletions) = authorChanges[authorName];
                    authorChanges[authorName] = (prevAdditions + additions, prevDeletions + deletions);
                }
                else
                {
                    authorChanges[authorName] = (additions, deletions);
                }
            }

            tableData = new List<List<object>>();

            foreach (var kvp in authorChanges)
            {
                var row = new List<object>();
                row.Add(kvp.key);
                row.Add(kvp.Value.additions);
                row.Add(kvp.Value.deletions);
                tableData.add(row);
            }

            ConsoleTableBuilder
                .From(tableData)
                .WithFormat(ConsoleTableBuilderFormat.Alternative)
                .ExportAndWriteLine(TableAlignment.Center);
        }
    }
}
