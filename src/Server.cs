RedisConfig.Role = "master";
RedisConfig.Port = 6379;
var argParser = new ParseCommandLineArguments().Parse(args);

foreach (var arg in argParser.args.Keys)
{
    switch (arg)
    {
        case "port":
            RedisConfig.Port = int.Parse(argParser.args[arg][1]);
            break;

        case "replicaof":
            RedisConfig.Role = "slave";
            break;
    }
}

Redis.Start(RedisConfig.Port);

class ParseCommandLineArguments
{
    public Dictionary<string, List<string>> args = new Dictionary<string, List<string>>();

    public ParseCommandLineArguments Parse(string[] argument)
    {
        var argValue = new List<string>();
        string key = "";
        for (int i = 0; i < argument.Length; i += 1)
        {
            if (argument[i].Contains("--"))
            {
                this.args.Add(argument[i].Trim('-'), argValue.Clone());
                key = argument[i].TrimStart('-');
            }
            else
            {
                argValue.Add(argument[i]);
            }
        }
        return this;
    }
}