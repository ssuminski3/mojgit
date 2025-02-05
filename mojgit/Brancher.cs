using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;

namespace mojgit
{
    class Brancher
    {
        FileManager fileManager;

        public Brancher(FileManager f)
        {
            this.fileManager = f;
        }
        public void createBranch(string name)
        {
            List<Branch> branches = new List<Branch>();
            string path = fileManager.getPath() + ".mojgit\\main.json";
           if (File.Exists(path))
           {
                Branch[] b = JsonSerializer.Deserialize<Branch[]>(File.ReadAllText(path));
                branches = b.ToList<Branch>();
           }
            branches.Add(new Branch { name = name });

            File.WriteAllText(path, JsonSerializer.Serialize(branches.ToArray()));
        }
    }
}
