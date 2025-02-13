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
        
        //Zapisanie commita w main.json
        public void commit(string branchName, string message, string commitName)
        {
            List<Branch> branches = new List<Branch>();

            int branchIndex = 0;

            Brancher brancher = new Brancher(fileManager);
            (branches, branchIndex) = brancher.findBranch(branchName);
            
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
            File.WriteAllText(fileManager.getPath() + ".mojgit\\main.json", JsonSerializer.Serialize(branches.ToArray(), options));
        }
    
        public int findCommit(string branchName, string commitName)
        {
            Brancher brancher = new Brancher(fileManager);

            List<Branch> branches = new List<Branch>();
            int branchIndex = 0;
            (branches, branchIndex) = brancher.findBranch(branchName);

            int commitIndex = Array.FindIndex<Commit>(branches[branchIndex].commits, item => item.name == commitName);

            return commitIndex;
        }
    }
}
