using System.Collections.Immutable;
using System.Net;
using System.Net.Sockets;
using System.Text;

// You can use print statements as follows for debugging, they'll be visible when running tests.
Console.WriteLine("Logs from your program will appear here!");

// Uncomment this block to pass the first stage
TcpListener server = new TcpListener(IPAddress.Any, 6379);
server.Start();
using TcpClient handler = await server.AcceptTcpClientAsync(); // wait for client
await using NetworkStream stream = handler.GetStream();
var message = $"+PONG\r\n";
var messageBytes = Encoding.UTF8.GetBytes(message);
await stream.WriteAsync(messageBytes);
handler.Close();
