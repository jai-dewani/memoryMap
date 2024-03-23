using System.Collections.Immutable;
using System.Net;
using System.Net.Sockets;
using System.Text;

// You can use print statements as follows for debugging, they'll be visible when running tests.
Console.WriteLine("Logs from your program will appear here!");

// Uncomment this block to pass the first stage
TcpListener server = new TcpListener(IPAddress.Any, 6379);
try
{

    server.Start();
    Console.WriteLine($"{server.LocalEndpoint.ToString()}");
    while (true)
    {
        using var handler = await server.AcceptSocketAsync(); // wait for client
        var message = $"+PONG\r\n";
        var messageBytes = Encoding.UTF8.GetBytes(message);
        handler.Send(messageBytes);
        byte[] data = new byte[256];
        handler.Receive(data);
        Console.WriteLine($"data - {data}");
    }
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}
finally
{
    server.Stop();
}


