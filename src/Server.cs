RedisConfig.Role = "master";
RedisConfig.Port = 6379;
RedisConfig.Offset = 0;
RedisConfig.ReplicationID = "8371b4fb1155b71f4a04d3e1bc3e18c4a990aeeb";
var argParser = new ParseCommandLineArguments().Parse(args);

foreach (var arg in argParser.args.Keys)
{
    switch (arg)
    {
        case "port":
            RedisConfig.Port = int.Parse(argParser.args[arg][0]);
            break;

        case "replicaof":
            RedisConfig.Role = "slave";
            RedisConfig.MasterHostUrl = argParser.args[arg][0];
            RedisConfig.MasterHostPort = int.Parse(argParser.args[arg][1]);
            break;
    }
}

new RedisClient().Start(RedisConfig.Port);

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
                this.args.Add(key, argValue.Clone());
                argValue = new List<string>();
                key = argument[i].TrimStart('-');
            }
            else
            {
                argValue.Add(argument[i]);
            }
        }
        this.args.Add(key,argValue.Clone());
        return this;
    }
}