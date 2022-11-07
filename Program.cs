using System;
using System.IO;
using System.Linq;

namespace PicDivide
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Arguments a empty. \r\nPlease provide two addresses ELI files.");
                return;
            }

            if (args.Length != 2)
            {
                Console.WriteLine("Please provide two addresses ELI files.");
                return;
            }

            if (!File.Exists(args[0]) || !File.Exists(args[1]))
            {
                Console.WriteLine("First or second files don't exist. \r\nMake sure that the provided path is correct.");
                return;
            }

            var files = args.Select(incomingPath => new EliFile(incomingPath)).ToList();
            var processor = new EliFileProcessor();

            var resultFile = processor.DivideImage(files[0], files[1]);

            var desctopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop); // не оговорено место сохранения, поэтому рабочий стол
            var fileName = "NewFile.ELI"; // принцип формирования имени не оговорен, поэтому просто NewFile
            var fullPath = Path.Combine(desctopPath, fileName);
            var writeTask = resultFile.MakeFileAsync(fullPath);

            if (writeTask.IsFaulted)
            {
                throw writeTask.Exception;
            }

            Console.WriteLine("Job is done!");
        }
    }
}
