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

            Console.WriteLine("Author Changes:");
            Console.WriteLine(new string('-', 40));
            Console.WriteLine("| Author".PadRight(20) + " | Additions | Deletions |");
            Console.WriteLine(new string('-', 40));

            foreach (var kvp in authorChanges)
            {
                Console.WriteLine($"| {kvp.Key.PadRight(20)} | {kvp.Value.additions.ToString().PadRight(9)} | {kvp.Value.deletions.ToString().PadRight(9)} |");
            }

            Console.WriteLine(new string('-', 40));
        }
    }
}
