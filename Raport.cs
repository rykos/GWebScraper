using System;
namespace webScraper
{
    public class Raport
    {
        public int CurrentFile = 1;
        public int AllFiles;


        public Raport(int allfiles)
        {
            this.AllFiles = allfiles;
        }

        public Raport(){}

        public void WriteRaport()
        {
            Console.WriteLine(string.Format("Processing file: {0}/{1}", CurrentFile, AllFiles));
        }

        public void FinishedFile(bool success = true)
        {
            Console.WriteLine("File {0}/{1} finished with {2}", CurrentFile, AllFiles, (success) ? "success" : "failure");
            this.CurrentFile++;
        }
    }
}