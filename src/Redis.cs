using System.Net;
using System.Net.Sockets;
using System.Text;

class Redis
{

    private static RedisKeyVault keyVault = new RedisKeyVault();
    public static void Start(int port = 6379)
    {
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

                    case "info":
                        response = RedisParser.Transform(RedisConfig.GetConfig(), StringType.BulkStrings);
                        break; 

                    case "get":
                        response = RedisParser.Transform(keyVault.Get(message[1]));
                        break;

                    case "set":
                        if (message.Count > 3)
                            keyVault.Set(message[1], RedisParser.Transform(message[2], int.Parse(message[4])));
                        else
                            keyVault.Set(message[1], RedisParser.Transform(message[2]));
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