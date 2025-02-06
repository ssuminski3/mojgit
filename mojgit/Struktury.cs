using System;

namespace mojgit
{
    public struct ChangedLines
    {
        public string line { get; set; }
        public int number { get; set; }
        public bool added { get; set; } // true if added, false if removed
    }

    public class FileChanges
    {
        public string fileName { get; set; }
        public ChangedLines[] changes { get; set; }
    }

    public struct Commit
    {
        public string name { get; set; }
        public string message { get; set; }
        public DateTime dateTime { get; set; }
        public FileChanges[] fileChanges { get; set; }
        public string[] files { get; set; }
    }
    //Because there is not possible to change it
    public class Branch
    {
        public string name { get; set; }
        public Commit[] commits { get; set; }
    }
}