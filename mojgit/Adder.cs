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
            List<ChangedLines> fileChanges = new List<ChangedLines>();
            if(f1.fileName == f2.fileName)
            {
                for(int i = 0; i <= f1.changes.Length-1; i++)
                {
                    if (!f2.changes.Any(item => f1.changes[i].Equals(item)))
                    {
                        fileChanges.Add(f1.changes[i]);
                    }
                }
                for (int i = 0; i <= f2.changes.Length - 1; i++)
                {
                    if (!f1.changes.Any(item => f2.changes[i].Equals(item)))
                    {
                        f2.changes[i].added = !f2.changes[i].added;
                        fileChanges.Add(f2.changes[i]);
                    }
                }
                if(fileChanges.Count > 0)
                {
                    ffinal.fileName = f1.fileName;
                    ffinal.changes = fileChanges.ToArray();
                }
            }
            return ffinal;
        }

        private List<FileChanges> CompareCommits(string branchName)
        {
            List<Branch> branches = new List<Branch>();
            List<FileChanges> ch = new List<FileChanges>();

            int branchIndex = 0;

            Brancher brancher = new Brancher(fileManager);
            (branches, branchIndex) = brancher.findBranch(branchName);

            foreach(Commit commit in branches[branchIndex].commits)
            {
                foreach(FileChanges fileChanges in commit.fileChanges)
                {
                    FileChanges f = changes.Find(item => item.fileName == fileChanges.fileName);
                    if(f != null)
                        ch.Add(CompareFileChanges(f, fileChanges));
                }
            }
            return ch;
        }

        private void CompareFiles(string file1, string file2)
        {
            List<ChangedLines> change = new List<ChangedLines>();
            string[] file1_lines = File.ReadAllLines(file1);
            string[] file2_lines = File.ReadAllLines(file2);

            HashSet<string> file2_set = new HashSet<string>(file2_lines);
            HashSet<string> file1_set = new HashSet<string>(file1_lines);

            for (int i = 0; i < file2_lines.Length; i++)
            {
                if (!file1_set.Contains(file2_lines[i]))
                {
                    change.Add(new ChangedLines { line = file2_lines[i], number = i + 1, added = false });
                }
            }

            for (int i = 0; i < file1_lines.Length; i++)
            {
                if (!file2_set.Contains(file1_lines[i]))
                {
                    change.Add(new ChangedLines { line = file1_lines[i], number = i + 1, added = true });
                }
            }

            if(change.Count > 0)
                changes.Add(new FileChanges { fileName = file1, changes = change.ToArray() });
        }

        public void CompareDirectories(string branch)
        { 

            List<string> list = fileManager.getPossiblyChangedFiles(fileManager.getPath(), fileManager.getPath() + ".mojgit\\legacy_code\\");
        
            for(int i = 0; i <= list.Count-1; i+=2)
            {
                CompareFiles(list[i], list[i + 1]);
            }
            changes = CompareCommits(branch);
            
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
}
