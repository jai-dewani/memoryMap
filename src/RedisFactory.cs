
class RedisFactory
{
    public static Command GetType(string[] args)
    {
        Console.WriteLine($"RedisFactory = {args.Length}, {String.Join("|", args)}");
        var mainCommand = args[0].ToLower();
        Console.WriteLine($"mainCommand = {mainCommand}");
        switch (mainCommand)
        {
            case "ping":
                return new PingCommand();

            case "echo":
                return new EchoCommand(args[1]);

            case "info":
                return new InfoCommand();

            case "get":
                return new GetCommand(args[1]);

            case "set":
                return new SetCommand(args[1], args[2], args.Count() > 3 ? int.Parse(args[4]) : null);

            case "replconf":
                return new ReplConfCommand();
                
            case "psync":
                return new PsyncCommand();
            default:
                System.Console.WriteLine($"Default Null Command");
                return new NullCommand();
        }
    }
}



abstract class Command { }

class NullCommand : Command {}

class PingCommand : Command
{
    public PingCommand(){
        Console.WriteLine("Ping Command");
    }
    public string response = "PONG";
}

class ReplConfCommand : Command 
{
    public string response = "OK";
}

class PsyncCommand : Command
{
    public string response = $"FULLRESYNC {RedisConfig.ReplicationID} {RedisConfig.Offset}";
}

class EchoCommand : Command
{
    public string response;

    public EchoCommand(string message)
    {
        this.response = message;
    }
}

class InfoCommand : Command
{
    public string response;

    public InfoCommand()
    {
        this.response = RedisConfig.GetConfig();
    }
}

class GetCommand : Command
{
    public string key;

    public GetCommand(string key)
    {
        this.key = key;
    }
}

class SetCommand : Command
{
    public string key;
    public string value;
    public int? expiryInMs;
    internal string response;

    public SetCommand(string key, string value, int? expiry)
    {
        this.key = key;
        this.value = value;
        this.expiryInMs = expiry;
        this.response = "OK";
    }
}