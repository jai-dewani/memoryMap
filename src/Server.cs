using System.Collections.Immutable;
using System.Net;
using System.Net.Sockets;
using System.Reflection.Metadata.Ecma335;
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
        var socket = server.AcceptSocket(); // wait for client
        var newThread = new Thread(() => Respond(socket));
        newThread.Start();
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


void Respond(Socket? socket)
{
    while (socket.Connected)
    {
        byte[] data = new byte[256];
        socket.Receive(data);
        string receivedMessage = Encoding.UTF8.GetString(data);
        Console.WriteLine($"data - {receivedMessage}");
        var message = $"+PONG\r\n";
        var messageBytes = Encoding.UTF8.GetBytes(message);
        socket.Send(messageBytes);
    }
}