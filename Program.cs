using git_pull;

//----

try
{
    Worker w = args.Length switch
    {
        0 => new( )
      , 1 => int.TryParse(args[0], out var levels) ? new(levels) : new(args[0])
      , 2 => new(args[0], int.Parse(args[1]))
      , _ => throw new ArgumentException("Incorrect arguments count")
    };

    w.Run( );
    w.Wait( );
}
catch (Exception e)
{
    Console.WriteLine(e);
}
finally
{
    Console.WriteLine("Done. Press any key to exit");
}

Console.ReadKey( );
