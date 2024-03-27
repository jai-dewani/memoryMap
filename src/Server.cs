RedisConfig.Role = "master";
RedisConfig.Port = 6379;
var argParser = new ParseCommandLineArguments().Parse(args);

foreach (var arg in argParser.args.Keys)
{
    switch (arg)
    {
        case "port":
            RedisConfig.Port = int.Parse(argParser.args[arg]);
            break;

        case "replicaof":
            RedisConfig.Role = "slave";
            break;
    }
}
Redis.Start(RedisConfig.Port);


class ParseCommandLineArguments
{
    public Dictionary<string, string> args = new Dictionary<string, string>();

    public ParseCommandLineArguments Parse(string[] argument)
    {
        for (int i = 0; i < argument.Length; i += 2)
        {
            this.args.Add(argument[i].Trim('-'), argument[i + 1]);
        }
        return this;
    }
}