class RedisParser
{
    public static string[] Parse(string input)
    {
        List<string> parsedCommands = new List<string>();
        var tokens = input.Split("\r\n");

        if (tokens[0].ElementAt(0) == '*')
        {
            // It's an Array
            for (int i = 2; i < tokens.Length; i += 2)
            {
                parsedCommands.Add(tokens[i]);
            }
        }
        else if (tokens[0].ElementAt(0) == '+')
        {
            // It's a simple string
            parsedCommands.Add(tokens[0].Substring(1));
        }
        return parsedCommands.ToArray();
    }

    public static string Transform(string message, StringType stringType)
    {
        switch (stringType)
        {
            case StringType.SimpleString:
                return $"+{message}\r\n";
            case StringType.BulkString:
                return $"${message.Length}\r\n{message}\r\n";
            case StringType.NullBulkString:
                return $"$-1\r\n";
            default:
                return $"{message}";
        }
    }

    public static string Transform(string[] messages)
    {
        var response = new List<string>();
        foreach(var message in messages)
        {
            response.Add(Transform(message, StringType.BulkString));
        }
        return $"*{messages.Length}\r\n{string.Join("\r\n", response)}";
    }
}
