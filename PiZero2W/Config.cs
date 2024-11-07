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

namespace PiZero2W;

using System.ComponentModel;
using ArtNet;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

public class Config
{
    public static string ConfigFile { get; private set; } = null!;

    public static Config Instance { get; private set; } = null!;

    public static void LoadConfigs()
    {
        generateOrLoadConfigs();
        if (Instance.Debug)
        {
            Program.Debug = true;
            ServerSettings.StaticDebug = true;
        }
        if (Instance.NeopixelDebug)
            Program.Debug = true;
        if (Instance.ArtnetDebug)
            ServerSettings.StaticDebug = true;
        Log.Object(Instance, "Config");
    }

    private static void generateOrLoadConfigs()
    {
        ConfigFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config.yaml");
        if (!File.Exists(ConfigFile))
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
        }
        catch (Exception)
        {
            Console.WriteLine($"Could not parse config. Generating new config.");
            GenerateNewConfigs();
        }
    }
    private static void GenerateNewConfigs()
    {
        File.Create(ConfigFile);
        var serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .DisableAliases()
            .Build();
        File.WriteAllText(ConfigFile, serializer.Serialize(new Config()));
        Instance = new();
    }

    public bool Debug { get; set; } = false;

    public bool ArtnetDebug { get; set; } = false;

    public bool NeopixelDebug { get; set; } = false;

    [DisplayName("16-Bit")]
    [Description("Enable this to enable 16 bit mode for the neopixels.")]
    public bool SixteenBit { get; set; } = false;

    [Description("Enables an intensity curve on the lower spectrum to make the pixels look darker at lower intensities without compromising brightness.")]
    public bool IntensityCurve { get; set; } = false;

    [DisplayName("4 Channel Mode")]
    [Description("Enables an intensity channel for dedicated intensity dimming.")]
    public bool FourChannelMode { get; set; } = false;

    [Description("DMX mappings provide a way to link Neopixels with DMX Addresses. [Dmx Address, [List of pixels]] ie. [0, [0, 1, 2, 3]], [1, [4, 5, 6, 7]]")]
    public Dictionary<ushort, List<ushort>> DMXMaps { get; set; } = new()
    {
        {
            1, new()
            {
                0,1,2,3,4,5,6,
            }
        },
        {
            4, new()
            {
                7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,
            }
        },
        {
            7, new()
            {
                23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,
            }
        }
    };
}