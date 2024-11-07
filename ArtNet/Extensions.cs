// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         PiZero2W
//    Project:          PiZero2W
//    FileName:         Extensions.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/12/2024 13:30
//    Created Date:     10/12/2024 13:30
// -----------------------------------------

namespace ArtNet;

using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using Packets;

public static class Extensions
{
    public static ushort GetNextUDPPort(int startingPort)
    {
        var properties = IPGlobalProperties.GetIPGlobalProperties();
        
        //getting active connections
        var tcpConnectionPorts = properties.GetActiveTcpConnections()
            .Where(n => n.LocalEndPoint.Port >= startingPort)
            .Select(n => n.LocalEndPoint.Port);

        //getting active tcp listners - WCF service listening in tcp
        var tcpListenerPorts = properties.GetActiveTcpListeners()
            .Where(n => n.Port >= startingPort)
            .Select(n => n.Port);
        
        //getting active udp listeners
        var udpListenerPorts = properties.GetActiveUdpListeners()
            .Where(n => n.Port >= startingPort)
            .Select(n => n.Port);

        // ReSharper disable once ReplaceWithSingleCallToFirstOrDefault
        var port = Enumerable.Range(startingPort, ushort.MaxValue)
            .Where(i => !tcpConnectionPorts.Contains(i))
            .Where(i => !tcpListenerPorts.Contains(i))
            .Where(i => !udpListenerPorts.Contains(i))
            .FirstOrDefault();
        
        return (ushort)port;
    }
    public static IPAddress? GetLocalIPAddress(List<Regex>? regex = null)
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        IPAddress? address = null;
        if (regex is null)
        {
            Regex defaultRegex = new("(192|10).*.*.*");
            address = host.AddressList.FirstOrDefault(x => defaultRegex.IsMatch(x.ToString())) ?? host.AddressList.FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork);
            
            return address;
        }
        
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily != AddressFamily.InterNetwork)
                continue;
            foreach (Regex search in regex)
            {
                if (search.IsMatch(ip.ToString()))
                    address = ip;
                break;
            }
        }
        
        Log.Debug($"Address {(address is null ? "is null" : $"Found: {address}")}", ServerSettings.StaticDebug);
        return address;
    }

    public static T[] GetEnumArray<T>(this byte[] byteArray) where T : Enum
    {
        T[] enums = new T[byteArray.Length];
        for (int i = 0; i < byteArray.Length; i++)
        {
            enums[i] = (T) Enum.ToObject(typeof(T), byteArray[i]);
        }
        return enums;
    }
}