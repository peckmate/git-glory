using System;
using System.IO;
using System.Linq;
using LibGit2Sharp;

public class Program
{
    public static void Main(string[] args)
    {
       var repositoryPath = Console.ReadLine();

        using (var repo = new Repository(repositoryPath))
        {
            var authorNames = new List<string>();

            foreach (var commit in repo.Commits)
            {
                authorNames.Add(commit.Author.Name);
            }

            var uniqueAuthorNames = authorNames.Distinct();

            Console.WriteLine("Unique Author Names:");

            foreach (var authorName in uniqueAuthorNames)
            {
                Console.WriteLine(authorName);
            }
        }
    }
}
