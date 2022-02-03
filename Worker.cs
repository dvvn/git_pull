using System.Diagnostics;

namespace git_pull;

internal class Worker : IDisposable
{
	private readonly List<Task> _tasks = new();
	private readonly int _maxLevel;
	private readonly string _rootPath;
	private readonly int _rootPathSkip;

	private async Task ExecuteCmd(string directory)
	{
		var procStartInfo = new ProcessStartInfo
		{
			WorkingDirectory = directory,
			FileName = "git",
			Arguments = "pull",
			RedirectStandardOutput = true,
			UseShellExecute = false,
			CreateNoWindow = true,
		};
		var proc = new Process
		{
			StartInfo = procStartInfo,
		};
		proc.Start();
		await proc.WaitForExitAsync();
		Console.WriteLine($"\"{directory[_rootPathSkip..]}\"{Environment.NewLine}{await proc.StandardOutput.ReadToEndAsync()}");
	}

	private static bool IsDirectoryValid(string directory)
	{
		return Directory.EnumerateDirectories(directory).Select(Path.GetFileName).Contains(".git");
	}

	private void Fill(string path, int level = 0)
	{
		foreach (var directory in Directory.EnumerateDirectories(path, "*", SearchOption.TopDirectoryOnly))
		{
			if (IsDirectoryValid(directory))
				_tasks.Add(ExecuteCmd(directory));
			else
			{
				if (level < _maxLevel)
					Fill(directory, level + 1);
			}
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

	public Worker(int levels = int.MaxValue) : this(Directory.GetCurrentDirectory(), levels)
	{
	}

	public void Fill()
	{
		Debug.Assert(_tasks.Count == 0);
		Fill(_rootPath);
	}

	public void Dispose()
	{
		Task.WhenAll(_tasks).Wait();
		_tasks.Clear();
	}

	public IEnumerable<Task> AsEnumerable()
	{
		return _tasks.AsEnumerable();
	}
}