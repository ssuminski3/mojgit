using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Linq;

namespace mojgit
{

    class Adder
    {
        private List<FileChanges> changes = new List<FileChanges>();
        FileManager fileManager;

        public Adder(FileManager f)
        {
            this.fileManager = f;
        }

        private FileChanges CompareFileChanges(FileChanges f1, FileChanges f2)
        {
            FileChanges ffinal = new FileChanges();

            if (f1.fileName == f2.fileName)
            {
                List<ChangedLines> fileChanges = new List<ChangedLines>();

                if (f1.changes != null && f2.changes != null)
                {
                    foreach (ChangedLines change in f1.changes)
                    {
                        int existsInF2 = f2.changes.ToList().FindIndex(c => c.Equals(change));
                        if (existsInF2 == -1)
                        {
                            fileChanges.Add(change);
                        }
                    }
                    foreach (ChangedLines change in f2.changes)
                    {
                        int existsInF1 = f1.changes.ToList().FindIndex(c => c.Equals(change));
                        if (existsInF1 == -1)
                        {
                            fileChanges.Add(new ChangedLines
                            {
                                line = change.line,
                                number = change.number,
                                added = false
                            });
                        }
                    }
                }

                if (fileChanges.Count > 0)
                {
                    ffinal.fileName = f1.fileName;
                    ffinal.changes = fileChanges.ToArray();
                }
            }

            return ffinal;
        }


        private void CompareCommits(string branchName)
        {
            List<Branch> branches = new List<Branch>();
            List<FileChanges> ch = new List<FileChanges>();

            int branchIndex = 0;

            Brancher brancher = new Brancher(fileManager);
            (branches, branchIndex) = brancher.findBranch(branchName);
            if(branches[branchIndex].commits != null)
                foreach(Commit commit in branches[branchIndex].commits)
                {
                    foreach(FileChanges fileChanges in commit.fileChanges)
                    {
                        FileChanges f = changes.Find(item => item.fileName == fileChanges.fileName);
                        if(f != null)
                            ch.Add(CompareFileChanges(f, fileChanges));
                    }
                }
            if (ch.Count() > 0)
                changes = ch;
        }

        private void CompareFiles(string file1, string file2)
        {
            string[] file1_lines = File.ReadAllLines(file1);
            string[] file2_lines = File.ReadAllLines(file2);
            int maxLines = Math.Max(file1_lines.Length, file2_lines.Length);
            List<ChangedLines> changesList = new List<ChangedLines>();

            for (int i = 0; i < maxLines; i++)
            {
                string line1 = i < file1_lines.Length ? file1_lines[i] : null;
                string line2 = i < file2_lines.Length ? file2_lines[i] : null;

                if (line1 != line2)
                {
                    // Możesz tutaj określić logikę, która rozróżnia dodane/usunięte linie
                    if (line1 == null)
                    {
                        // linia dodana w file2
                        changesList.Add(new ChangedLines { line = line2, number = i + 1, added = false });
                    }
                    else if (line2 == null)
                    {
                        // linia usunięta z file1
                        changesList.Add(new ChangedLines { line = line1, number = i + 1, added = true });
                    }
                    else
                    {
                        // linie się różnią – można dodać obie zmiany lub zaznaczyć, że linia została zmodyfikowana
                        changesList.Add(new ChangedLines { line = line1, number = i + 1, added = true });
                        changesList.Add(new ChangedLines { line = line2, number = i + 1, added = false });
                    }
                }
            }

            if (changesList.Count > 0)
                changes.Add(new FileChanges { fileName = file1.Replace(fileManager.getPath(), ""), changes = changesList.ToArray() });
        }

        public void CompareDirectories(string branch)
        { 

            List<string> list = fileManager.getPossiblyChangedFiles(fileManager.getPath(), fileManager.getPath() + ".mojgit\\legacy_code\\");
            for(int i = 0; i <= list.Count-1; i+=2)
            {
                CompareFiles(list[i], list[i + 1]);
            }
            CompareCommits(branch);
        }


        public void DisplayChanges()
        {
            foreach (FileChanges change in changes)
            {
                Console.WriteLine($"Name: {change.fileName}: "); 
                if(change.changes != null)
                    foreach (ChangedLines ch in change.changes)
                    {
                        if(!ch.Equals(new ChangedLines()))
                            Console.WriteLine($"    {ch.number}: {(ch.added ? "Added" : "Removed")} -> {ch.line}");
                    }
            }
        }
    
        public void ParseToJSON(string path)
        {
            Console.WriteLine(changes.ToString());
            if (changes.Count() > 0)
            {
                if (changes[0].changes != null)
                {
                    Console.WriteLine(changes[0].changes.ToString());
                    FileChanges[] files = changes.ToArray();
                    Console.WriteLine(files.ToString());
                    var options = new JsonSerializerOptions
                    {
                        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    };
                    string json = JsonSerializer.Serialize(files, options);
                    Console.WriteLine(json);
                    File.WriteAllText(path + ".mojgit\\add.json", json);
                    Console.WriteLine("Added");
                }
            }
            else
                Console.WriteLine("Brak zmian");
        }

    }
}
