RedisConfig.Role = "master";
if (args.Length == 0)
{
    Redis.Start();
}
else
{
    Console.Write("Application Args - ");
    foreach (string arg in args)
    {
        Console.Write($"{arg} ");
    }
    Redis.Start(int.Parse(args[1]));
}
