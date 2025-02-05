using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Encodings.Web;

namespace mojgit
{

    class Adder
    {
        private List<FileChanges> changes = new List<FileChanges>();

        private void CompareFiles(string file1, string file2)
        {
            List<ChangedLines> change = new List<ChangedLines>();
            string[] file1_lines = File.ReadAllLines(file1);
            string[] file2_lines = File.ReadAllLines(file2);

            HashSet<string> file2_set = new HashSet<string>(file2_lines);

            for (int i = 0; i < file1_lines.Length; i++)
            {
                if (!file2_set.Contains(file1_lines[i]))
                {
                    change.Add(new ChangedLines { line = file1_lines[i], number = i + 1, added = false });
                }
            }

            HashSet<string> file1_set = new HashSet<string>(file1_lines);

            for (int i = 0; i < file2_lines.Length; i++)
            {
                if (!file1_set.Contains(file2_lines[i]))
                {
                    change.Add(new ChangedLines { line = file2_lines[i], number = i + 1, added = true });
                }
            }
            if(change.Count > 0)
                changes.Add(new FileChanges { fileName = file1, changes = change.ToArray() });
        }

        public void CompareDirectories(List<string> list)
        {
            for(int i = 0; i <= list.Count-1; i+=2)
            {
                CompareFiles(list[i], list[i + 1]);
            }
        }

        public void DisplayChanges()
        {
            foreach (FileChanges change in changes)
            {
                Console.WriteLine($"Name: {change.fileName}: "); 
                foreach (ChangedLines ch in change.changes)
                {
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
