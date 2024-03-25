using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;


Redis.Start();
// foreach(var st in RedisParser.Parse("*2\r\n$4\r\necho\r\n$3\r\nhey\r\n"))
// {
//     Console.WriteLine(st);
// }

class Redis
{

    private static RedisKeyVault keyVault = new RedisKeyVault();
    public static void Start()
    {
        TcpListener server = new TcpListener(IPAddress.Any, 6379);
        try
        {
            server.Start();
            while (true)
            {
                var socket = server.AcceptSocket(); // wait for client
                var newThread = new Thread(() => Respond(socket));
                newThread.Start();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception! Message - ${ex.Message}");
        }
        finally
        {
            server.Stop();
        }

        void Respond(Socket? socket)
        {
            while (socket.Connected)
            {
                byte[] data = new byte[4000];
                socket.Receive(data, data.Length, SocketFlags.None);
                string receivedMessage = Encoding.UTF8.GetString(data);
                Console.WriteLine($"data - {receivedMessage}");
                var message = RedisParser.Parse(receivedMessage);
                string response;
                switch (message[0])
                {
                    case "echo":
                        response = RedisParser.Transform(message[1], StringType.BulkStrings);
                        break;

                    case "ping":
                        response = RedisParser.Transform("PONG", StringType.SimpleStrings);
                        break;

                    case "get":
                        response = RedisParser.Transform(keyVault.Get(message[1]));
                        break;

                    case "set":
                        keyVault.Set(message[1], RedisParser.Transform(message[2], int.Parse(message[4])));
                        response = RedisParser.Transform("OK", StringType.SimpleStrings);
                        break;

                    default:
                        response = "";
                        break;
                }
                socket.Send(Encoding.UTF8.GetBytes(response));
            }
        }
    }
}

class RedisParser
{
    public static List<string> Parse(string input)
    {
        List<string> parsedCommands = new List<string>();
        var tokens = input.Split("\r\n");


        if (tokens[0].ElementAt(0) == '*')
        {
            // It's an Array
            int length = int.Parse(tokens[0].Substring(1));
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
        Console.WriteLine($"Parsed Commands");
        foreach (var token in parsedCommands)
            Console.WriteLine(token);
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
            return $"${message.value.Length}\r\n{message}\r\n";
    }

    public static RedisValueModel Transform(string message, int? timeout)
    {
        DateTime? expiry = timeout != null ? DateTime.Now.AddMilliseconds(timeout.Value) : null;
        return new RedisValueModel(message, expiry);
    }
}

public enum StringType
{
    SimpleStrings,
    BulkStrings,
    SimpleErrors,
}

public class RedisKeyVault
{
    private Dictionary<string, RedisValueModel> store = new Dictionary<string, RedisValueModel>();

    public RedisValueModel Get(string key)
    {
        return store[key];
    }

    public void Set(string key, RedisValueModel value)
    {
        store[key] = value;
    }
}

public class RedisValueModel
{
    public string value { get; protected set; }
    public DateTime? expiry { get; protected set; }

    public RedisValueModel(string value, DateTime? expiry)
    {
        this.value = value;
        this.expiry = expiry;
    }
}