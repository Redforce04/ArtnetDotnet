// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         PiZero2W
//    Project:          PiZero2W
//    FileName:         Config.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/12/2024 17:21
//    Created Date:     10/12/2024 17:21
// -----------------------------------------

namespace MosfetDriver;

using System.ComponentModel;
using System.IO.Ports;
using ArtNet;
using Enums;
using Spectre.Console;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

public class Config
{
    public static string ConfigFile { get; private set; } = null!;

    public static Config Instance { get; private set; } = null!;

    public static void LoadConfigs()
    {
        // Instance = new();
        generateOrLoadConfigs();
    }

    private static void generateOrLoadConfigs()
    {
        ConfigFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config.yaml");
        if (!File.Exists(ConfigFile))
        {
            GenerateNewConfigs();
            return;
        }
        string configText = File.ReadAllText(ConfigFile);
        if (configText.Length < 3)
        {
            GenerateNewConfigs();
            return;
        }
        try
        {
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .WithCaseInsensitivePropertyMatching()
                .Build();
            Instance = deserializer.Deserialize<Config>(File.ReadAllText(ConfigFile));
            if (Instance is null)
            {
                GenerateNewConfigs();
            }
            AnsiConsole.MarkupLine("[yellow][[Config]][/] Config loaded.");
        }
        catch (Exception)
        {
            AnsiConsole.MarkupLine($"[yellow][[Config]][/] Could not parse config. Generating new config.");
            GenerateNewConfigs();
        }
    }
    private static void GenerateNewConfigs()
    {
        try
        {
            AnsiConsole.MarkupLine("[yellow][[Config]][/] : Generating new configs...");
            if(!File.Exists(ConfigFile))
               File.Create(ConfigFile).Close();
            var serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .DisableAliases()
                .Build();
            File.WriteAllText(ConfigFile, serializer.Serialize(new Config()));
        }
        catch (Exception e)
        {
            AnsiConsole.WriteException(e);
        }
        Instance = new();
    }

    [Description("Enables debug logging for the base server.")]
    public bool Debug { get; set; } = false;
     
    [Description("Enables debug logging for artnet server.")]
    public bool ArtNetDebug { get; set; } = false;

    [Description("The starting Universe. Only 1 - 16 will work due to artnet server implementation.")]
    public ushort StartUniverse { get; set; } = 16;

    [Description("The starting Address.")]
    public ushort StartAddress { get; set; } = 1;

    [Description("Should 16bit mode be used.")]
    public bool SixteenBit { get; set; } = false;
    
    [Description("How many ports should be used.")]
    public byte Ports { get; set; } = 4;

    
    
    [Description("The type of connection to use.")]
    public ConnectionType ConnectionType { get; set; } = ConnectionType.Serial;
    
    

    [Description("The SPI Bus to use.")]
    public short SPIBusId { get; set; } = 1;
    [Description("The SPI Chip Select to use.")]
    public short SPIChipSelect { get; set; } = 1;
    
    
    [Description("The I2C Bus to use.")]
    public short I2CBusId { get; set; } = 1;
    [Description("The I2C Device Address to use.")]
    public short I2CDeviceAddress { get; set; } = 1;
    
    
    

    [Description("The Serial Port to use.")]
    public string SerialPort { get; set; } = "Com1";
    [Description("The Serial Baud Rate to use.")]
    public int SerialBaudRate { get; set; } = 9600;
    [Description("The Serial Parity to use. Options: None (no bit), Even (even parity), Odd (odd parity), Mark (always 1), Space (always 0)")]
    public Parity SerialParity { get; set; } = Parity.Even;
    [Description("The Serial Data Bits to use.")]
    public ushort SerialDataBits { get; set; } = 8;
    [Description("The Serial Stop Bits to use. Options: None, One, Two, OnePointFive")]
    public StopBits SerialStopBits { get; set; } = StopBits.One;
}
