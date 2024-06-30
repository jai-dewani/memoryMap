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
            int port = int.Parse(argParser.args[arg][0]);
            System.Console.WriteLine($"Port = {port}");
            RedisConfig.Port = port;
            break;

        case "replicaof":
            RedisConfig.Role = "slave";
            RedisConfig.MasterHostUrl = argParser.args[arg][0];
            RedisConfig.MasterHostPort = int.Parse(argParser.args[arg][1]);
            break;
    }
}

new RedisClient().Start(RedisConfig.Port);