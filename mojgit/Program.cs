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
            //fileManager.CopyDirectory(path, path + ".mojgit\\legacy_code");
            Brancher brancher = new Brancher(fileManager);
            brancher.createBranch("main");
            Adder adder = new Adder(fileManager);
            adder.CompareDirectories("main");
            adder.DisplayChanges();
            adder.ParseToJSON(path);
            //adder.DisplayChanges();

            Commiter commiter = new Commiter(fileManager);
            commiter.commit("main", "ZOBACZMY", "1.0");

            Backer backer = new Backer(fileManager);
            
            backer.GetBack("main", "1.0");
        }
    }
}
