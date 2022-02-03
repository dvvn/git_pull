using git_pull;

//----

static Worker Init(IReadOnlyList<string> args)
{
	return args switch
	{
		{Count: 0} => new Worker(),
		{Count: 1} => int.TryParse(args[0], out var levels) ? new Worker(levels) : new Worker(args[0]),
		{Count: 2} => new Worker(args[0], int.Parse(args[1])),
		_ => throw new ArgumentException("Incorrect arguments")
	};
}

//----

try
{
	using var w = Init(args);
	w.Fill();
}
catch (Exception e)
{
	Console.WriteLine(e);
}
finally
{
	Console.WriteLine("Done. Press any key to exit");
}

Console.ReadKey();