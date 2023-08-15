using System;
using System.IO;
using System.Linq;
using LibGit2Sharp;
using System.Collections.Generic;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Enter the path to the repository: ");
        string repositoryPath = Console.ReadLine();

        using (var repo = new Repository(repositoryPath))
        {
            var authorChanges = new Dictionary<string, (int additions, int deletions)>();

            foreach (var commit in repo.Commits)
            {
                var changes = commit.Parents.Any()
                    ? repo.Diff.Compare<TreeChanges>(commit.Parents.First().Tree, commit.Tree)
                    : repo.Diff.Compare<TreeChanges>(null, commit.Tree);

                string authorName = commit.Author.Name;

                int additions = changes.Sum(changes => change.LinesAdded);
                int deletions = changes.Sum(changes => changes.LinesDeleted);


                if (authorChanges.ContainsKey(authorName))
                {
                    var (prevAdditions, prevDeletions) = authorChanges[authorName];
                    authorChanges[authorName] = (prevAdditions + additions, prevDeletions + deletions);
                }
                else
                {
                    authorChanges[authorName] = (additions, deletions)
                }
            }

            Console.WriteLine("Author Changes:");

            foreach (var kvp in authorChanges)
            {
                Console.WriteLine($"{kvp.Key}: {kvp.Value.additions} additions, {kvp.Value.deletions} deletions");
            }
        }
    }
}
