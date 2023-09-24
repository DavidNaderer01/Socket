using System.Net.Sockets;
using System.Net;
using System.Text;

IPEndPoint ipEndPoint = new(IPAddress.Loopback, 11_000);
using Socket client = new(
    ipEndPoint.AddressFamily,
    SocketType.Stream,
    ProtocolType.Tcp);

await client.ConnectAsync(ipEndPoint);
while (true)
{
    // Send message.
    Console.Write("Enter message: ");
    var message = $"{Console.ReadLine()!}<|EOM|>";
    var messageBytes = Encoding.UTF8.GetBytes(message);
    _ = await client.SendAsync(messageBytes, SocketFlags.None);
    Console.WriteLine($"Socket client sent message: \"{message.Replace("<|EOM|>", "")}\"");

    // Receive ack.
    var buffer = new byte[1_024];
    var received = await client.ReceiveAsync(buffer, SocketFlags.None);
    var response = Encoding.UTF8.GetString(buffer, 0, received);
    if (response == "<|ACK|>")
    {
        Console.WriteLine(
            $"Message send");
    }

    if (message.Contains("end"))
        break;
}

client.Shutdown(SocketShutdown.Both);