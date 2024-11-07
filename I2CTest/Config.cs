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

namespace UnitTests;

using System.ComponentModel;
using Spectre.Console;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

public class Config
{
    public static string ConfigFile { get; private set; } = null!;

    public static Config Instance { get; private set; } = null!;

    public static void LoadConfigs()
    {
        Instance = new();
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
        try
        {
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .WithCaseInsensitivePropertyMatching()
                .Build();
            Instance = deserializer.Deserialize<Config>(File.ReadAllText(ConfigFile));
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
            File.Create(ConfigFile);
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
    [Description("The starting Universe. Only 1 - 16 will work due to artnet server implementation.")]
    public ushort StartUniverse { get; set; } = 16;

    [Description("The starting Address.")]
    public ushort StartAddress { get; set; } = 1;
}
