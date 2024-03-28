class Redis
{
    private RedisKeyVault keyVault = new RedisKeyVault();
    public string Execute(Command command)
    {
        switch (command)
        {
            case PingCommand pingCommand:
                return this.Ping(pingCommand);
            case EchoCommand echoCommand:
                return this.Echo(echoCommand);
            case InfoCommand infoCommand:
                return this.Info(infoCommand);
            case GetCommand getCommand:
                return this.Get(getCommand);
            case SetCommand setCommand:
                return this.Set(setCommand);
            default:
                return "";
        }
    }

    private string Ping(PingCommand pingCommand)
    {
        return RedisParser.Transform(pingCommand.response, StringType.SimpleString);
    }
    private string Echo(EchoCommand echoCommand)
    {
        return RedisParser.Transform(echoCommand.response, StringType.BulkString);
    }
    private string Info(InfoCommand infoCommand)
    {
        return RedisParser.Transform(infoCommand.response, StringType.BulkString);
    }
    private string Get(GetCommand getCommand)
    {
        var value = this.keyVault.Get(getCommand.key);
        if (value.expiry != null && DateTime.Now > value.expiry)
            return RedisParser.Transform("", StringType.NullBulkString);
        else
            return RedisParser.Transform(value.value, StringType.BulkString);
    }
    private string Set(SetCommand setCommand)
    {
        DateTime? expiry = setCommand.expiryInMs != null
            ? DateTime.Now.AddMilliseconds(setCommand.expiryInMs.Value)
            : null;
        RedisValueModel redisValueModel = new RedisValueModel(setCommand.value, expiry);
        this.keyVault.Set(setCommand.key, redisValueModel);
        return RedisParser.Transform(setCommand.response, StringType.SimpleString);
    }

}

