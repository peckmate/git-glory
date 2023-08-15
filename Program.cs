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
        string repositoryPath = Console.Readline();

        using (var repo = new Repository(repositoryPath));
        {
            var authorCommitCounts = new Dictionary<string, int>();

            foreach (var commit in repo.Commits)
            {
                if (authorCommitCounts.ContainsKey(authorName))
                {
                    authorCommitCounts[authorName]++;
                }
                else
                {
                    authorCommitCounts[authorName] = 1;
                }
            }

            Console.WriteLine("Author Commit Counts:");

            foreach (var kvp in authorCommitCounts)
            {
                console.WriteLine($"{kvp.Key}: {kvp.Value} commits");
            }
        }
    }
}
