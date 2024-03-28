using System.Net;
using System.Net.Sockets;
using System.Text;

class RedisClient
{
    public Redis redis = new Redis();
    public async void Start(int port)
    {
        if(RedisConfig.MasterHostUrl != null && RedisConfig.MasterHostPort.HasValue)
        {
            using var master = new TcpClient(RedisConfig.MasterHostUrl, RedisConfig.MasterHostPort.Value);
            await using var stream = master.GetStream();
            await using var writer = new StreamWriter(stream);
            await writer.WriteAsync(RedisParser.Transform(new string[]{"ping"}));
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

    void Respond(Socket? socket)
    {
        while (socket.Connected)
        {
            byte[] rawByteArgs = new byte[4000];
            socket.Receive(rawByteArgs, rawByteArgs.Length, SocketFlags.None);

            string rawArgs = Encoding.UTF8.GetString(rawByteArgs);

            var args = RedisParser.Parse(rawArgs);
            Console.WriteLine(string.Join(" ", args));

            var command = RedisFactory.GetType(args);
            var response = this.redis.Execute(command);
            socket.Send(Encoding.UTF8.GetBytes(response));
        }
    }
}