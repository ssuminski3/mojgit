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
        }
        
        public void CopyDirectory(string sourceDir, string destinationDir, string[] list = null)
        {
            //Console.WriteLine(sourceDir);
            if (sourceDir == path + ".mojgit")
                return;

            if (!Directory.Exists(destinationDir))
            {
                Directory.CreateDirectory(destinationDir);
            }

            foreach (string file in Directory.GetFiles(sourceDir))
            {
                Console.WriteLine("COPY: " + file);
                try
                {
                    string destFile = Path.Combine(destinationDir, Path.GetFileName(file));
                    if (list != null)
                    {
                        if (list.Any(item => item == file))
                        {
                            File.Copy(file, destFile, false);
                        }
                    }
                    else
                    {
                        File.Copy(file, destFile, false);
                        Console.WriteLine("OKEJ");
                    }
                }
                catch
                {
                    Console.WriteLine("Plik istnieje");
                }
            }

            foreach (string subDir in Directory.GetDirectories(sourceDir))
            {
                string newDestinationDir = Path.Combine(destinationDir, Path.GetFileName(subDir));
                //Console.WriteLine(newDestinationDir);
                CopyDirectory(subDir, newDestinationDir);
            }
        }
    
        public List<string> getPossiblyChangedFiles(string directory1, string directory2)
        {
            List<string> possiblyChangedFiles = new List<string>();
            foreach (string file1 in Directory.GetFiles(directory1))
            {
                foreach (string file2 in Directory.GetFiles(directory2))
                {

                    if (file1 == file2.Replace(".mojgit\\legacy_code\\", ""))
                    {
                        Console.WriteLine("FILE: " + file1);
                        Console.WriteLine("FILE: " + file2);
                        possiblyChangedFiles.Add(file1);
                        possiblyChangedFiles.Add(file2);
                    }
                }
            }
            foreach (string subdir1 in Directory.GetDirectories(directory1))
            {
                foreach (string subdir2 in Directory.GetDirectories(directory2))
                {
                    if(subdir1 == subdir2.Replace(".mojgit\\legacy_code\\", ""))
                        possiblyChangedFiles.AddRange(getPossiblyChangedFiles(subdir1, subdir2));
                }
            }
            return possiblyChangedFiles;
        }
    
        public void commitFiles()
        {
            CopyDirectory(path, path + ".mojgit\\legacy_code");
        }
        
        public string getPath()
        {
            return path;
        }
        
        public string[] getAllFiles(string p)
        {
            List<string> files = new List<string>();

            foreach(string f in Directory.GetFiles(p))
            {
                
                List<string> file = f.Split("\\").ToList<string>();
                if (file.Contains(".mojgit"))
                    continue;
                string filename = file[file.Count() - 1];
                Console.WriteLine("CO: " + filename);
                file.Remove(filename);
                file.Add(".mojgit\\legacy_code\\" + filename);
                string path = string.Join("\\", file);
                Console.WriteLine("PATH: " + path);
                files.Add(path);
            }

            foreach(string d in Directory.GetDirectories(p))
            {
                getAllFiles(d);
            }

            return files.ToArray();
        }
    }
}
