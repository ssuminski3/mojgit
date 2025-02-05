using System;
using System.IO;
using System.Collections.Generic;

namespace mojgit
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = "C:\\Users\\sebas\\OneDrive\\Pulpit\\TestowyProjektmojgita\\";
            //ToDo Dynamiczne zmienianie śćieżki przez odpalanie programu w danych ścieżkach
            FileManager fileManager = new FileManager(path);
            List<string> comparableFiles = fileManager.getPossiblyChangedFiles(path, path + ".mojgit\\legacy_code\\");
            Adder adder = new Adder();
            //adder.CompareDirectories(comparableFiles);
            //adder.DisplayChanges();
            //adder.ParseToJSON(path);
            //adder.DisplayChanges();
            Brancher brancher = new Brancher(fileManager);
            brancher.createBranch("main");

            Commiter commiter = new Commiter(fileManager);
            commiter.commit("main", "Powinno działać", "0.1");

        }
    }
}
