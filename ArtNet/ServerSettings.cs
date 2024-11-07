// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         PiZero2W
//    Project:          ArtNet
//    FileName:         ServerSettings.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/13/2024 13:11
//    Created Date:     10/13/2024 13:11
// -----------------------------------------

namespace ArtNet;

using System.Net;
using System.Text.RegularExpressions;
using Structs;

public class ServerSettings
{
    /// <summary>
    /// Enables Debug for static methods and extensions such as get ip address.
    /// </summary>
    public static bool StaticDebug { get; set; }

    /// <summary>
    /// Sets the duration of failed data transfers before a new address should be bound.
    /// </summary>
    public uint ResetNetworkAdapterAfterXDataFailures = 10;
        
    /// <summary>
    /// Gets or sets a list of ArtNet Universes that the server will listen for. 
    /// </summary>
    public List<ArtNetUniverse> UniversesToListenFor { get; set; } = new();

    /// <summary>
    /// Gets or sets regex searches for valid addresses to bind to.
    /// </summary>
    public List<Regex> ValidAddresses { get; set; } = new()
    {
        new Regex("192.*"),
        new Regex("10.*")
    };

/// <summary>
    /// Gets or sets the port for the server to listen to.
    /// </summary>
    public ushort Port { get; set; } = 6454;

    /// <summary>
    /// Gets or sets the mac address to report over artnet.
    /// </summary>
    public byte[] MacAddress { get; set; } = new byte[6];

    /// <summary>
    /// Gets or sets a value that indicates whether the sequence header should be ignored or not. When ignoring the sequence header, old frames can be processed out of order.
    /// </summary>
    public bool IgnoreSequenceHeader { get; set; }

    /// <summary>
    /// Determines whether or not debug information should be logged.
    /// </summary>
    public bool DebugMode { get; set; }

    /// <summary>
    /// Gets or sets the amount of seconds of no data before the server should send an empty frame of data, to turn devices off. Default is 3, Disable this feature with -1.
    /// </summary>
    // ReSharper disable once CompareOfFloatsByEqualityOperator
    public float DataLossSeconds { get => this._dataLossSeconds; set => this._dataLossSeconds = (value == -1) ? -1 : Math.Clamp(value, 0, float.MaxValue); }

    private float _dataLossSeconds = 3;

    public ServerSettings SetMacAddress(byte[] mac)
    {
        this.MacAddress = mac;
        return this;
    }
    
    /// <summary>
    /// Adds a valid address or family to search for. * is supported. <seealso cref="ValidAddresses"/>
    /// </summary>
    /// <param name="address">The address to search for.</param>
    public ServerSettings AddAddress(string address)
    {
        ValidAddresses.Add(new Regex(address));
        return this;
    }

    /// <summary>
    /// Sets the delay before the server sends an empty frame of data from the last packet. 
    /// </summary>
    /// <param name="delay">The delay in seconds.</param>
    public ServerSettings DataLossTime(float delay)
    {
        DataLossSeconds = delay;
        return this;
    }

    /// <summary>
    /// Disables the delay before the server sends an empty frame of data from the last packet. 
    /// </summary>
    public ServerSettings DisableDataLoss()
    {
        DataLossSeconds = -1;
        return this;
    }
    
    /// <summary>
    /// Adds a valid address to search for. <seealso cref="ValidAddresses"/>
    /// </summary>
    /// <param name="address">The address to search for.</param>
    public ServerSettings AddIPAddress(IPAddress address)
    {
        ValidAddresses.Add(new Regex(address.ToString()));
        return this;
    }
    
    /// <summary>
    /// Adds a valid address search item to search for .<seealso cref="ValidAddresses"/>
    /// </summary>
    /// <param name="regex">The address search regex.</param>
    public ServerSettings AddIPAddress(Regex regex)
    {
        ValidAddresses.Add(regex);
        return this;
    }
    
    /// <summary>
    /// Sets <see cref="DebugMode"/> to true. Debug information will be logged to the console.
    /// </summary>
    public ServerSettings EnableDebug(bool enable = true)
    {
        DebugMode = enable;
        return this;
    }
    
    /// <summary>
    /// Sets the <see cref="Port"/> to listen to.
    /// </summary>
    /// <param name="port">The port that the server should listen too.</param>
    public ServerSettings WithCustomPort(ushort port)
    {
        this.Port = port;
        return this;
    }

    /// <summary>
    /// Sets the <see cref="IgnoreSequenceHeader"/> to true. Can lead to old frames being processed out of order.
    /// </summary>
    public ServerSettings SkipSequenceHeaderCheck()
    {
        this.IgnoreSequenceHeader = true;
        return this;
    }
    
    /// <summary>
    /// Adds an address for the server to listen for. The address is added to <see cref="UniversesToListenFor"/>.
    /// </summary>
    /// <param name="universe">The universe of the address.</param>
    /// <param name="address">The local address.</param>
    public ServerSettings AddAddress(ushort universe, ushort address)
    {
        var addr = new DMXAddress((ushort)(universe), address);
        if (this.UniversesToListenFor.Contains(addr.ArtNetUniverse))
            return this;
        
        this.UniversesToListenFor.Add(addr.ArtNetUniverse);
        return this;
    }

    /// <summary>
    /// Adds an address for the server to listen for. The address is added to <see cref="UniversesToListenFor"/>.
    /// </summary>
    /// <param name="fullAddress">The full address to add.</param>
    public ServerSettings AddAddress(uint fullAddress)
    {
        var addr = new DMXAddress(fullAddress);
        if (this.UniversesToListenFor.Contains(addr.ArtNetUniverse))
            return this;
        
        this.UniversesToListenFor.Add(addr.ArtNetUniverse);
        return this;
    }
    
    /// <summary>
    /// Adds a list of full addresses for the server to listen for. The addresses are added to <see cref="UniversesToListenFor"/>.
    /// </summary>
    /// <param name="fullAddresses">The list of full addresses to add.</param>
    public ServerSettings AddAddresses(List<uint> fullAddresses)
    {
        foreach (var fullAddress in fullAddresses)
        {
            var addr = new DMXAddress(fullAddress);
            if (this.UniversesToListenFor.Contains(addr.ArtNetUniverse))
                return this;
        
            this.UniversesToListenFor.Add(addr.ArtNetUniverse);
        }
        return this;
    }
    
    /// <summary>
    /// Adds a list of addresses for the server to listen for. The addresses are added to <see cref="UniversesToListenFor"/>.
    /// </summary>
    /// <param name="addresses">The list of addresses to add.</param>
    public ServerSettings AddAddresses(List<DMXAddress> addresses)
    {
        foreach (var address in addresses)
        {
            if (this.UniversesToListenFor.Contains(address.ArtNetUniverse))
                return this;
        
            this.UniversesToListenFor.Add(address.ArtNetUniverse);
        }
        return this;
    }

    /// <summary>
    /// Adds an ArtNet universe for the server to listen to.
    /// </summary>
    /// <param name="universe">The universe to listen to.</param>
    public ServerSettings AddUniverse(ArtNetUniverse universe)
    {
        if (!this.UniversesToListenFor.Contains(universe))
            this.UniversesToListenFor.Add(universe);
        return this;
    }
}