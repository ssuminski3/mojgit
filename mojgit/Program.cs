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
            Adder adder = new Adder(fileManager);
            adder.CompareDirectories("main");
            adder.DisplayChanges();
            adder.ParseToJSON(path);
            //adder.DisplayChanges();
            //Brancher brancher = new Brancher(fileManager);
            //brancher.createBranch("main");

            Commiter commiter = new Commiter(fileManager);
            commiter.commit("main", "Następny", "0.2");

        }
    }
}
