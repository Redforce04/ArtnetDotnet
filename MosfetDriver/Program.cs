namespace MosfetDriver;

// ReSharper disable FunctionNeverReturns
using System.Device.I2c;
using System.Device.Spi;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using ArtNet;
using ArtNet.Structs;
using Enums;
using Newtonsoft.Json;
using Spectre.Console;
using Exception = System.Exception;

public class Program
{
    
#region Variables
// ReSharper disable once UnusedAutoPropertyAccessor.Global
    public static Program Instance { get; private set; } = null!;
    private static ushort ListeningAddressCount { get; set; } = 2;
    public static Server ArtNetServer { get; private set; } = null!;
    internal I2cDevice I2cSender { get; }
    internal SpiDevice SpiSender { get; }
    internal SerialPort SerialSender { get; }
#endregion

public static void Main(string[] args)
{
    Log.Info($"Starting at [[{DateTime.Now:G}]]");
    Config.LoadConfigs();
    try
    {

        while (true)
        {
            Log.Debug($"Looking for addresses.", Config.Instance.Debug);
            Thread.Sleep(1000);
            var host = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress? address = null;

            foreach (var ip in host.AddressList)
            {
                Log.Debug($"Ip address: {ip.ToString()}");
                if (ip.AddressFamily != AddressFamily.InterNetwork)
                    continue;
                if (!ip.ToString().StartsWith("192") && !ip.ToString().StartsWith("10.1"))
                    continue;
                address = ip;
            }

            if (address != null)
                break;
            Log.Debug($"Address {(address is null ? "is null" : $"Found: {address}")}", Config.Instance.Debug);
        }
    }
    catch (Exception ex)
    {
        Log.Exception(ex);
    }

    ServerSettings.StaticDebug = Config.Instance.ArtNetDebug;
    var settings = new ServerSettings();
    settings.DebugMode = Config.Instance.ArtNetDebug;
    ListeningAddressCount = (ushort)(Config.Instance.Ports * (Config.Instance.SixteenBit ? 2 : 1));
    Log.Debug($"Config {Config.Instance.StartUniverse}/{Config.Instance.StartAddress} ({Config.Instance.Ports} @ {(Config.Instance.SixteenBit ? "16 Bit": "8 Bit")}) [[{ListeningAddressCount}]]", Config.Instance.Debug);

    for (byte i = 0; i < Config.Instance.Ports; i++)
    {
        if (Config.Instance.SixteenBit)
        {
            
            Log.Debug($"Subscribed to address {Config.Instance.StartUniverse}/{Config.Instance.StartAddress + i * 2}", Config.Instance.Debug);
            Log.Debug($"Subscribed to address {Config.Instance.StartUniverse}/{Config.Instance.StartAddress + 1 + i * 2}", Config.Instance.Debug);
            settings.AddAddress((ushort)(Config.Instance.StartUniverse -1), (ushort) (Config.Instance.StartAddress + i * 2));
            settings.AddAddress((ushort)(Config.Instance.StartUniverse -1), (ushort) (Config.Instance.StartAddress + 1 + i * 2));
            continue;
        }
        Log.Debug($"Subscribed to address {Config.Instance.StartUniverse}/{Config.Instance.StartAddress + i}", Config.Instance.Debug);
        settings.AddAddress((ushort)(Config.Instance.StartUniverse -1), (ushort) (Config.Instance.StartAddress + i));
    }
    ArtNetServer = new Server(
        settings
    );
    new Program().MainLoop(args);
} 
    private Program()
    {
        Instance = this; 
        Log.Debug($"Connection Type: {Config.Instance.ConnectionType}", Config.Instance.Debug);
        switch (Config.Instance.ConnectionType)
        { 
            case ConnectionType.SPI:
                SpiSender = SpiDevice.Create(new SpiConnectionSettings(Config.Instance.SPIBusId, Config.Instance.SPIChipSelect));
                break;
            case ConnectionType.I2C:
                I2cSender = I2cDevice.Create(new I2cConnectionSettings(Config.Instance.I2CBusId, Config.Instance.I2CDeviceAddress));
                break;
            case ConnectionType.Serial:
                SerialSender = new SerialPort(Config.Instance.SerialPort, Config.Instance.SerialBaudRate, Config.Instance.SerialParity, Config.Instance.SerialDataBits, Config.Instance.SerialStopBits);
                break;
        }
    }

    // ReSharper disable once UnusedParameter.Local
    private void MainLoop(string[] args)
    {
        ArtNetServer.Run();
        SerialSender.Open();
        byte[] bytes = new byte [ListeningAddressCount];
        
        while (true)
        {
            try
            {
                if (!ArtNetServer.Data.TryDequeue(out DMXUniverseData? data))
                {
                    Thread.Sleep(25);
                    continue;
                }
                // ReSharper disable PossibleLossOfFraction
                /*if (data.Universe.Universe != (Config.Instance?.StartUniverse ?? 16))
                {
                    AnsiConsole.MarkupLine($"[cyan][[Info]][/] Universe {data.Universe.Universe}.");            
                    continue;
                }*/

                // AnsiConsole.MarkupLine($"[[Artnet]] Data Len: ({data.Data.Count}).");
                if (data.Data.Count < ListeningAddressCount)
                {
                    Log.Debug($"Data missing. ({data.Data.Count} < {ListeningAddressCount})");
                    continue;
                }
                for (int i = 0; i < ListeningAddressCount; i++)
                {
                    if (Config.Instance.SixteenBit)
                    {
                        // Remember that the start array of [0] is actually dmx address 1.
                        bytes[i * 2] = data.Data[(byte)(Config.Instance.StartAddress - 1 + i * 2)].Data;
                        bytes[i * 2 + 1] = data.Data[(byte)(Config.Instance.StartAddress + 0 + i * 2)].Data;
                        continue;
                    }
                    bytes[i] = data.Data[(byte)(Config.Instance.StartAddress + i -1)].Data;
                }

                switch (Config.Instance.ConnectionType)
                {
                    case ConnectionType.SPI:
                        SpiSender.Write(bytes );
                        break;
                    case ConnectionType.I2C:
                        I2cSender.Write(bytes );
                        break;
                    case ConnectionType.Serial:
                        string sendStr = "";
                        foreach (byte b in bytes)
                        {
                            sendStr += b + " ";
                        }
                        SerialSender.WriteLine(sendStr);
                        break;
                }
                string list = "";
                for (int i = 0; i < bytes.Length; i++)
                {
                    list += $"{i+1}: {bytes[i]}, ";
                }
                Log.Debug($" {list.Substring(0, list.Length-2)} [[{data.Universe.Subnet.ToString()}/{data.Universe.Universe.ToString()} ({data.Universe.FullUniverse.ToString()})]]", Config.Instance.Debug);
            }
            catch (Exception e)
            {
                Log.Exception(e, $"Issue caught while trying to process DMX Data.");
                
            }
        }
    }
}
