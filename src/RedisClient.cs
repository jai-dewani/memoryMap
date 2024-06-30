using System.Net;
using System.Net.Sockets;
using System.Text;

class RedisClient
{
    public Redis redis = new Redis();
    public void Start(int port)
    {
        if (RedisConfig.MasterHostUrl != null)
        {
            Console.WriteLine("Replica Server, connecting to master");
            this.ConnectToMaster();
        }
        TcpListener server = new TcpListener(IPAddress.Any, port);
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
    }

    void Respond(Socket socket)
    {
        while (socket.Connected)
        {
            byte[] rawByteArgs = new byte[4000];
            socket.Receive(rawByteArgs, rawByteArgs.Length, SocketFlags.None);

            string rawArgs = Encoding.UTF8.GetString(rawByteArgs).Trim('\0').Trim('$');
            Console.WriteLine($"rawArgs recevied from socket - ${rawArgs}");


            var args = RedisParser.Parse(rawArgs);
            Console.WriteLine($"args - {String.Join(' ', args)}");

            var command = RedisFactory.GetType(args);
            var response = this.redis.Execute(command);
            Console.WriteLine($"Response = {response}");
            var encodedResponse = Encoding.UTF8.GetBytes(response);
            socket.Send(encodedResponse);
        }
    }

    async void ConnectToMaster()
    {
        var masterRedisServer = new TcpClient(RedisConfig.MasterHostUrl, RedisConfig.MasterHostPort);
        NetworkStream stream = masterRedisServer.GetStream();

        var pingMessage = RedisParser.Transform("ping", StringType.SimpleString);
        var pingMessageBytes = Encoding.UTF8.GetBytes(pingMessage);
        Console.WriteLine($"Ping message bytes = {pingMessage}");
        stream.Write(pingMessageBytes);

        Console.WriteLine($"Ping command sent, waiting for the response");

        byte[] rawByteArgs = new byte[4000];
        await stream.ReadAsync(rawByteArgs);
        string rawArgs = Encoding.UTF8.GetString(rawByteArgs);
        Console.WriteLine($"Master response: {rawArgs}");

        var replconf1 = RedisParser.Transform(new string[] {"REPLCONF", "listening-port", $"{RedisConfig.Port}"});
        stream.Write(Encoding.UTF8.GetBytes(replconf1));

        await stream.ReadAsync(rawByteArgs);
        rawArgs = Encoding.UTF8.GetString(rawByteArgs);
        Console.WriteLine($"Master response: {rawArgs}");

        var replconf2 = RedisParser.Transform(new string[] { "REPLCONF", "capa", "psync2"});
        stream.Write(Encoding.UTF8.GetBytes(replconf2));

        await stream.ReadAsync(rawByteArgs);
        rawArgs = Encoding.UTF8.GetString(rawByteArgs);
        Console.WriteLine($"Master response: {rawArgs}");

        var psync = RedisParser.Transform(new string[] {"PSYNC", "?", "-1"});
        stream.Write(Encoding.UTF8.GetBytes(psync));
    
    }
}