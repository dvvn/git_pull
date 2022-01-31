// See https://aka.ms/new-console-template for more information

using git_pull;

Worker.MakeTask(/*Directory.GetCurrentDirectory()*/"C:\\Programming\\Projects\\git").Wait();
Console.WriteLine("Done. Press any key to exit");
Console.ReadKey();