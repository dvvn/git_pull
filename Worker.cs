using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace git_pull;

internal class Worker
{
	private static async Task ExecuteCmd(string directory)
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
		Console.WriteLine($"{directory}{Environment.NewLine}{ await proc.StandardOutput.ReadToEndAsync()}");
	}

	private static bool IsDirectoryValid(string directory)
	{
		return Directory.EnumerateDirectories(directory).Select(Path.GetFileName).Contains(".git");
	}

	public static IEnumerable<Task> MakeTasks(string path, string searchPattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly)
	{
		foreach (var directory in Directory.EnumerateDirectories(path, searchPattern, searchOption))
		{
			if (IsDirectoryValid(directory))
			{
				yield return ExecuteCmd(directory);
			}
			else
			{
				foreach (var task in MakeTasks(directory, searchPattern, searchOption))
					yield return task;
			}
		}
	}

	public static async Task MakeTask(string path, string searchPattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly)
	{
		await Task.WhenAll(MakeTasks(path));
	}
}