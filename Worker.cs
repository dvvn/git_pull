using System.Diagnostics;

namespace git_pull;

internal class Worker
{
    private readonly List<Task> _tasks = new( );
    private readonly int _maxLevel;
    private readonly string _rootPath;
    private readonly int _rootPathSkip;

    private Task _currentTask = Task.CompletedTask;

    private const string AppName = "git";
    private const string Arg = "pull";
    private const string TargetFolder = ".git";

    private async Task ExecuteCmd(string directory)
    {
        var procStartInfo = new ProcessStartInfo
        {
            WorkingDirectory = directory
          , FileName = AppName
          , Arguments = Arg
          , RedirectStandardOutput = true
          , UseShellExecute = false
          , CreateNoWindow = true
        };
        using var proc = new Process { StartInfo = procStartInfo, };
        if (!proc.Start( ))
            throw new($"Unable to start the {AppName} process");
        await proc.WaitForExitAsync( );
        Console.WriteLine($"\"{directory[_rootPathSkip..]}\"{Environment.NewLine}{await proc.StandardOutput.ReadToEndAsync( )}");
    }

    private static bool IsDirectoryValid(string directory)
    {
        return Directory.EnumerateDirectories(directory).Select(Path.GetFileName).Contains(TargetFolder);
    }

    private void Fill(string path, int level = 0)
    {
        foreach (var directory in Directory.EnumerateDirectories(path, "*", SearchOption.TopDirectoryOnly))
        {
            if (IsDirectoryValid(directory))
                _tasks.Add(ExecuteCmd(directory));
            else if (level < _maxLevel)
                Fill(directory, level + 1);
        }
    }

    public Worker(string path, int levels = int.MaxValue)
    {
        Debug.Assert(levels > 0);
        _rootPath = path;
        _rootPathSkip = path.Length;
        if (!Path.EndsInDirectorySeparator(path))
            ++_rootPathSkip;
        _maxLevel = levels;
    }

    public Worker(int levels = int.MaxValue)
        : this(Directory.GetCurrentDirectory( ), levels)
    {
    }

    public void Wait( )
    {
        _currentTask.Wait( );
    }

    public void Run( )
    {
        Debug.Assert(_tasks.Count == 0);
        Fill(_rootPath);
        Debug.Assert(_currentTask.IsCompleted);
        _currentTask = Task.WhenAll(_tasks);
    }

    public void Clear( )
    {
        Wait( );
        _tasks.Clear( );
    }
}
