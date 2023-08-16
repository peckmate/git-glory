using System; 
using System.IO; 
using System.Linq; 
using LibGit2Sharp; 
using System.Collections.Generic;
using ConsoleTableExt;
using System.ComponentModel.Design;

public class Program 
{ 
    public static void Main(string[] args) 
    { 
        while (true) 
        { 
            Console.WriteLine("Menu:"); 
            Console.WriteLine("1. Count Changes"); 
            Console.WriteLine("0. Exit"); 
            Console.Write("Select an option: "); 

            int option = int.Parse(Console.ReadLine());

            switch(option)
            { 
                case 1: 
                    Console.WriteLine("Give me a path");
                    var repositoryPath = Console.ReadLine(); 
                    CountChanges(repositoryPath); 
                    break; 
                
                case 0: 
                    return; 

                default: 
                    Console.WriteLine("Invalid option. Try again."); 
                    break; 
            }
        }
    }

    public static void CountChanges(string repositoryPath) 
    { 
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

            var tableData = new List<List<object>>(); 

            foreach (var kvp in authorChanges) 
            { 
                var row = new List<object>(); 
                row.Add(kvp.Key); 
                row.Add(kvp.Value.additions); 
                row.Add(kvp.Value.deletions);
                tableData.Add(row); 
            }

            ConsoleTableBuilder
                .From(tableData)
                .WithTitle("Ch-ch-changes", ConsoleColor.Yellow, ConsoleColor.DarkGray)
                .WithColumn("Name", "Additions", "Deletions")
                .ExportAndWriteLine();

        }
    }
}