namespace Testing_Program;

using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text.RegularExpressions;
using ArtNet;
using ArtNet.Structs;
using Spectre.Console;

class Program
{
    private const bool ShouldRunServer = true;
    static void Main(string[] args)
    {
        if (ShouldRunServer)
        {
            RunServer();
        }
        else
        {
            StartListener();
            ListAllResources();
            SendUDP();
        }
    }

    private static void RunServer()
    {
        var settings = new ServerSettings().AddUniverse(new ArtNetUniverse(1)).EnableDebug().AddIPAddress(new Regex("192.*")).SetMacAddress([0x30, 0x03, 0xC8, 0xE7, 0xE4, 0xD5]);
        var server = new Server(settings);
        server.Run();
        while (true)
        {
            // if (Server.Logs.TryDequeue(out string? log))
            // Console.WriteLine(log);
            // TestContext.Out.WriteLine(log);
            
            if (!server.Data.TryDequeue(out DMXUniverseData? data))
                continue;
            AnsiConsole.WriteLine($"data received: {data.Data.Count}");
            // Console.WriteLine($"data received: {data.Data.Count}");
            // data.Data;
        }
        // ReSharper disable once FunctionNeverReturns
    }

    private static void StartListener()
    {
        Thread listener = new (Listener);
        listener.Start();
    }
    private static void Listener()
    {
        const ushort listeningPort = 6454;
        const string listeningIP = "192.168.1.31";
        IPEndPoint localEndpoint = new (IPAddress.Parse(listeningIP), listeningPort);
        IPEndPoint remoteEndpoint = new (IPAddress.Any, 0);
        UdpClient client = new();
        client.Client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
        client.Client.Bind(localEndpoint);
        AnsiConsole.WriteLine($"Listener Binding to Address {localEndpoint.Address}:{localEndpoint.Port}");
        Thread.Sleep(1000);
        while (true)
        {
            byte[] receive = client.Receive(ref remoteEndpoint);
            AnsiConsole.WriteLine($"Received Packet [{receive.Length}] from {remoteEndpoint.Address}:{remoteEndpoint.Port}");
        }
        // ReSharper disable once FunctionNeverReturns
    }
    private static void SendUDP()
    {
        //Socket socket = new Socket(SocketType.Dgram, ProtocolType.Udp);
        const ushort localPort = 50000;
        const string localIp = "192.168.1.31";
        const ushort destPort = 6454;
        const string destIp = "192.168.1.31";
        //socket.Bind(new IPEndPoint(IPAddress.Parse(ip), port));
        IPEndPoint localEndpoint = new(IPAddress.Parse(localIp), localPort);
        IPEndPoint remoteEndpoint = new(IPAddress.Parse(destIp), destPort);
        UdpClient client = new();
        client.Client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
        client.Client.Bind(localEndpoint);
        AnsiConsole.WriteLine($"Sender Binding to Address {localEndpoint.Address}:{localEndpoint.Port}");
        Thread.Sleep(1000);
        while (true)
        {
            
            byte[] packet = GetEmbeddedHexPacket("ArtPollReply.hex");
            client.Send(packet, packet.Length, remoteEndpoint);
            AnsiConsole.WriteLine($"Sending Packet. [{packet.Length}] ({(IPEndPoint)client.Client.LocalEndPoint} -> {destIp}:{destPort})");
            // socket.Send(packet, SocketFlags.None);
            
            Thread.Sleep(1500);
        }
        // ReSharper disable once FunctionNeverReturns
    }

    public static void ListAllResources()
    {
        var names = Assembly.GetExecutingAssembly().GetManifestResourceNames();
        string list = $"Files Found [{names.Length}]\n";
        for (int i = 0; i < names.Length; i++)
        {
            list += $"  - {names[i]}\n";
        }
        AnsiConsole.WriteLine(list);
    }
    
    public static byte[] GetEmbeddedBinaryPacket(string fileName)
    {
        List<byte> bytes = new();
        Stream? stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Testing_Program.EmbeddedResources." + fileName);
        if (stream is null)
            return bytes.ToArray();
        while (stream.CanRead)
        {
            bytes.Add((byte)stream.ReadByte());
        }
        return bytes.ToArray();
    }

    public static byte[] GetEmbeddedHexPacket(string fileName)
    {
        List<byte> byteArray = new();
        Stream? stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Testing_Program.EmbeddedResources." + fileName);
        if (stream is null)
            return byteArray.ToArray();
        StreamReader reader = new(stream);
        string text = reader.ReadToEnd();
        string[] lines = text.Split('\n');
        foreach (string line in lines)
        {
            string[] bytes = line.Split(' ');
            for (int i = 0; i < bytes.Length; i++)
            {
                string Byte = bytes[i];
                // Header or spacer
                if (Byte.Length != 2)
                    continue;
                byteArray.Add(byte.Parse(Byte, NumberStyles.HexNumber));
            }
        }
        return byteArray.ToArray();
    }
}
