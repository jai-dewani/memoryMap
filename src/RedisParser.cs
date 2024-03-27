class RedisParser
{
    public static List<string> Parse(string input)
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
        return parsedCommands;
    }

    public static string Transform(string message, StringType stringType)
    {
        switch (stringType)
        {
            case StringType.SimpleStrings:
                return $"+{message}\r\n";
            case StringType.BulkStrings:
                return $"${message.Length}\r\n{message}\r\n";
            default:
                return $"{message}";
        }
    }

    public static string Transform(RedisValueModel message)
    {
        if (message.expiry != null && DateTime.Now > message.expiry)
            return $"$-1\r\n";
        else
            return $"${message.value.Length}\r\n{message.value}\r\n";
    }

    public static RedisValueModel Transform(string message, int? timeout = null)
    {
        DateTime? expiry = timeout != null ? DateTime.Now.AddMilliseconds(timeout.Value) : null;
        return new RedisValueModel(message, expiry);
    }

    public static string Transform(List<string> messages, StringType bulkStrings)
    {
        string response = "";
        foreach(var message in messages)
        {
            response += Transform(message,bulkStrings);
        }
        Console.WriteLine($"Info message - {response}");
        return response;
    }
}
