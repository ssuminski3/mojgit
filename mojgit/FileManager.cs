using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace mojgit
{
    class FileManager
    {
        string path;

        public FileManager(string path)
        {
            this.path = path;
            CreateFile("add.json");
            CreateFile("main.json");
            CopyDirectory(path, path + ".mojgit\\legacy_code");
        }
        private void CreateFile(string name)
        {
            Directory.CreateDirectory(path + ".mojgit\\legacy_code");
            File.Create(path + ".mojgit\\" + name);
        }
        
        private void CopyDirectory(string sourceDir, string destinationDir)
        {
            //Console.WriteLine(sourceDir);
            if (sourceDir == path + ".mojgit")
            {
                Console.WriteLine("STOP");
                return;
            }

            if (!Directory.Exists(destinationDir))
            {
                Directory.CreateDirectory(destinationDir);
            }

            foreach (string file in Directory.GetFiles(sourceDir))
            {
                try
                {
                    string destFile = Path.Combine(destinationDir, Path.GetFileName(file));
                    File.Copy(file, destFile, false);
                }
                catch
                {
                    Console.WriteLine("Plik istnieje");
                }
            }

            foreach (string subDir in Directory.GetDirectories(sourceDir))
            {
                string newDestinationDir = Path.Combine(destinationDir, Path.GetFileName(subDir));
                Console.WriteLine(newDestinationDir);
                CopyDirectory(subDir, newDestinationDir);
            }
        }
    }
}
