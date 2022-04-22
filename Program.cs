﻿using git_pull;

//----

static Worker Init(IReadOnlyList<string> args)
{
    return args.Count switch
    {
        0 => new()
      , 1 => int.TryParse(args[0], out var levels) ? new(levels) : new(args[0])
      , 2 => new(args[0], int.Parse(args[1]))
      , _ => throw new ArgumentException("Incorrect arguments count")
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
