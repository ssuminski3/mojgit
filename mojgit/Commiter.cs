using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using System.Text.Encodings.Web;


namespace mojgit
{
    class Commiter
    {
        public FileManager fileManager;

        public Commiter(FileManager f)
        {
            this.fileManager = f;
            f.commitFiles();
        }

        private Commit createCommit(string n, string m)
        {
            string file = File.ReadAllText(fileManager.getPath() + ".mojgit\\add.json");

            FileChanges[] fileChanges = JsonSerializer.Deserialize<FileChanges[]>(file);

            return new Commit 
            { 
                name = n, 
                message = m, 
                dateTime = new DateTime(), 
                fileChanges = fileChanges, 
                files = fileManager.getAllFiles(fileManager.getPath()) 
            };
        }

        public void commit(string name, string message, string commitName)
        {
            List<Branch> branches = new List<Branch>();
            string path = fileManager.getPath() + ".mojgit\\main.json";
            
            Branch[] b = JsonSerializer.Deserialize<Branch[]>(File.ReadAllText(path));
            branches = b.ToList<Branch>();

            int branchIndex = Array.FindIndex(branches.ToArray(), br => br.name == name);

            List<Commit> commitList = new List<Commit>();
            if(branches[branchIndex].commits != null)
            {
                commitList = branches[branchIndex].commits.ToList();
            }
            commitList.Add(createCommit(commitName, message));
            branches[branchIndex].commits = commitList.ToArray();


            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            };
            File.WriteAllText(path, JsonSerializer.Serialize(branches.ToArray(), options));
        }
    }
}
