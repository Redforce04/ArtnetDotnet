namespace PiZero2W;

// ReSharper disable FunctionNeverReturns
using System.Collections.Immutable;
using System.Device.Spi;
using System.Drawing;
using ArtNet;
using ArtNet.Structs;
using Iot.Device.Ws28xx;
using YamlDotNet.Serialization;

public class Program
{
#region ConstSettings
    internal static Dictionary<ushort, ushort> NeopixelArrays { get; } = new()
    {
        { 0, 16 },
        { 1, 16 },
        { 2, 7 },
    };
    internal static ushort NeopixelCount { get; } = (ushort) NeopixelArrays.Sum(x => x.Value);
#endregion
#region Variables
    public static bool Debug { get; internal set; } = false;
    public static Program Instance { get; private set; } = null!;

    internal SpiDevice Spi { get; }

    internal Ws2812b NeoPixels { get; }

    internal RawPixelContainer Image => NeoPixels.Image;
#endregion

private static void GenerateAddressFile()
{
    Dictionary<uint, int> addressesDict = new();
    for (int i = 0; i < 38; i++)
    {
       addressesDict.Add(new DMXAddress(11, (ushort)((i * 3) + 1)).FullAddress, i); 
       // addresses.Add(new DMXAddress(11, (ushort)((i * 3) + 2))); 
       // addresses.Add(new DMXAddress(11, (ushort)((i * 3) + 3))); 
    }
    SerializerBuilder builder = new ();
    var serializer = builder.Build();
    string x = serializer.Serialize(addressesDict);
    File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "outputAddresses.yaml"), x);
}
public static void Main(string[] args)
{
    GenerateAddressFile();
    ProcessArgs(args);
    Config.LoadConfigs();
    new Program().MainLoop(args);
} 
    private Program()
    {
        Instance = this;
        // Create connection settings to connect to the panel using SPI
        SpiConnectionSettings settings = new(0, 0)
        {
            ClockFrequency = 2_400_000,
            Mode = SpiMode.Mode0,
            DataBitLength = Config.Instance.SixteenBit ? 16 : 8,
        };

        // Create an SPI device
        this.Spi = SpiDevice.Create(settings);   
        
        // Use the SPI device to connect to the LEDs
        // and pass the number of LEDs
        this.NeoPixels = new Ws2812b(this.Spi, NeopixelCount);
    }

    private void MainLoop(string[] args)
    {
        Log.Debug("Flashing White", Debug);
        PixelTools.SetColorAll(Color.FromArgb(5,5,5));
        PixelTools.SetColorAll(Color.Black);
        Thread.Sleep(500);
        
        var server = new Server(new ServerSettings().EnableDebug(ServerSettings.StaticDebug));
        server.Run(); 
        
        Log.Debug("Cascading Blue", Debug);
        PixelTools.CascadeColor(Color.FromArgb(0,0,5), 50);
        PixelTools.CascadeColor(Color.Black, 50);

        while (true)
        {
            try
            {
                // todo - rework the byte array and also replace dmx maps.
                // Make enums for neopixel "modules" that are supported. Offer 1:1 or 1:all mapping for the modules or for the whole thing.
                // Modules could have "multi-cell modules" or dmx profiles built in for more options than 1:1 or 1:all. ie. 1-cell, 3-cell, 6-cell, 12-cell.
                // then the starting addresses could be updated.
                if (!server.Data.TryDequeue(out DMXUniverseData? data))
                {
                    Thread.Sleep(25);
                    continue;
                }
                if (Config.Instance.DMXMaps.Count < 1)
                {
                    PixelTools.SetColorAll(data.Data[0].Data, data.Data[1].Data, data.Data[2].Data);
                    if (server.Data.Count > 3)
                    {
                        server.Data.Clear();
                    }
                    continue;
                }
                foreach (var KVP in Config.Instance.DMXMaps)
                {
                    foreach (ushort neopixel in KVP.Value)
                    {
                        this.Image.SetPixel(neopixel, 0, Color.FromArgb(data.Data[(byte)(KVP.Key - 1)].Data, data.Data[(byte)KVP.Key].Data, data.Data[(byte)(KVP.Key + 1)].Data));
                    }
                }
                this.NeoPixels.Update();
                if (server.Data.Count > 3)
                {
                    server.Data.Clear();
                }
                
            }
            catch (Exception e)
            {
                Console.WriteLine($"Issue caught while trying to process DMX Data. Error:\n{e}");
            }
        }
    }

    
    private static void ProcessArgs(string[] args)
    {
        foreach (string arg in args)
        {
            switch (arg.ToLower())
            {
                case "-debug-artnet":
                    ArtNet.ServerSettings.StaticDebug = true;
                    Console.WriteLine($"Debugging artnet.");
                    break;
                case "-debug-neopixels":
                    Program.Debug = true;
                    Console.WriteLine($"Debugging neopixels.");
                    break;
                case "-debug":
                    ServerSettings.StaticDebug = true;
                    Program.Debug = true;
                    Console.WriteLine($"Debugging all program elements.");
                    break;
                
            }
        }
    }
}
