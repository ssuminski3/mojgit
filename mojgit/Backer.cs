using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace mojgit
{
    class Backer
    {
        private FileManager fileManager;

        public Backer(FileManager f)
        {
            this.fileManager = f;
        }

        public void ApplyChanges(FileChanges fileChanges, string commitName)
        {
            if (!string.IsNullOrEmpty(fileChanges.fileName) && fileChanges.changes != null)
            {
                string legacyCodePath = Path.Combine(fileManager.getPath(), commitName);
                string sourceFilePath = Path.Combine(legacyCodePath, fileChanges.fileName);
                string targetFilePath = sourceFilePath;


                List<string> result = File.Exists(sourceFilePath)
                    ? File.ReadAllLines(sourceFilePath).ToList()
                    : new List<string>();

                var sortedChanges = fileChanges.changes
                    .OrderBy(c => c.number)
                    .ThenBy(c => c.added) 
                    .ToList();

                foreach (var change in sortedChanges)
                {
                    int index = change.number - 1;

                    if (change.added)
                    {

                        if (index >= 0 && index <= result.Count)
                        {
                            result.Insert(index, change.line);
                        }
                        else
                        {
                            while(index >= result.Count)
                            {
                                result.Add("");
                            }
                            result.Insert(index, change.line);
                        }
                    }
                    else
                    {                        
                        result.Remove(change.line);
                    }
                }

                File.WriteAllLines(targetFilePath, result);
            }
        }


        public void GetBack(string branchName, string commitName)
        {
            Commiter commiter = new Commiter(fileManager);
            int commitIndex = commiter.findCommit(branchName, commitName);

            Brancher brancher = new Brancher(fileManager);
            List<Branch> branches;
            int branchIndex;
            (branches, branchIndex) = brancher.findBranch(branchName);

            string[] list = branches[branchIndex].commits[branchIndex].files;

            fileManager.CopyDirectory(fileManager.getPath() + ".mojgit\\legacy_code", fileManager.getPath() + commitName, list);

            for (int i = 0; i <= commitIndex; i++)
            {
                foreach (FileChanges fileChanges in branches[branchIndex].commits[i].fileChanges)
                {
                    ApplyChanges(fileChanges, commitName);
                }
            }
        }
    }
}
