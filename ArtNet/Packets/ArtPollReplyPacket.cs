// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         PiZero2W
//    Project:          ArtNet
//    FileName:         OpPollReply.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/14/2024 9:48
//    Created Date:     10/14/2024 9:48
// -----------------------------------------

namespace ArtNet.Packets;

using System.Net;
using Enums;
using Structs;

public class ArtPollReplyPacket : ArtNetPacket
{
    internal ArtPollReplyPacket(IPAddress address, ushort port, ushort artnetVersion, byte netSwitch, byte subSwitch,
        ushort oemId, byte ubeaVersion, Status status1, ushort estaId, string portName, string longName, string nodeReport,
        byte numberOfPorts, ArtNetPort[] portTypes, ArtNetInputPortStatus[] goodInputs, ArtNetOutputPortStatus[] goodOutputs,
        byte[] swIn, byte[] swOut, byte acnPriority, ArtNetMacro swMacro, ArtNetRemote swRemote, StyleCode style, byte[] mac,
        byte[]? bindIpAddress = null, byte? bindIndex = null, Status2? status2 = null, ArtNetOutputPortStatus2[]? goodOutputB = null, Status3? status3 = null,
        byte[]? defaultResponseUid = null, ushort? user = null, ushort? refreshRate = null, BackgroundQueuePolicy? backgroundQueuePolicy = null, byte[]? filler = null) : base()
    {
        this.IpAddress = address;
        this.Port = port;
        this.Version = artnetVersion;
        this.NetSwitch = netSwitch;
        this.SubSwitch = subSwitch;
        this.OemId = oemId;
        this.UbeaVersion = ubeaVersion;
        this.Status1 = (Status)status1;
        this.EstaId = estaId;
        this.PortName = portName.Length > 18 ? portName.ToCharArray().Take(18).ToArray() : portName.ToCharArray();
        this.LongName = longName.Length > 64 ? longName.ToCharArray().Take(64).ToArray() : longName.ToCharArray();
        this.NodeReport = nodeReport.Length > 64 ? nodeReport.ToCharArray().Take(64).ToArray() : nodeReport.ToCharArray();
        this.NumberOfPorts = numberOfPorts;
        this.PortTypes = portTypes.Length > 4 ? portTypes.Take(4).ToArray() : portTypes;
        for (int i = PortTypes!.Length; i < 4; i++)
        {
            PortTypes = PortTypes.Append((ArtNetPort)0).ToArray();
        }
        this.GoodInput = goodInputs.Length > 4 ? goodInputs.Take(4).ToArray() : goodInputs;
        for (int i = GoodInput!.Length; i < 4; i++)
        {
            GoodInput = GoodInput.Append((ArtNetInputPortStatus)0).ToArray();
        }
        this.GoodOutput = goodOutputs.Length > 4 ? goodOutputs.Take(4).ToArray() : goodOutputs;
        for (int i = GoodOutput!.Length; i < 4; i++)
        {
            GoodOutput = GoodOutput.Append((ArtNetOutputPortStatus)0).ToArray();
        }
        this.SwIn = swIn.Length > 4 ? swIn.Take(4).ToArray() : swIn;
        this.SwOut = swOut.Length > 4 ? swOut.Take(4).ToArray() : swOut;
        this.ACNPriority = acnPriority;
        this.SwMacro = swMacro;
        this.SwRemote = swRemote;
        this.Spare = [0, 0, 0];
        this.Style = (StyleCode) style;
        this.Mac = mac.Length > 6 ? mac.Take(6).ToArray() : mac;
        
        this.BindIPAddress = (bindIpAddress is not null && bindIpAddress.Length > 4) ? bindIpAddress.Take(4).ToArray() : bindIpAddress;
        this.BindIndex = bindIndex;
        this.Status2 = status2;
        this.GoodOutputB = (goodOutputB is not null && goodOutputB.Length > 4) ? goodOutputB.Take(4).ToArray() : goodOutputB;
        if (GoodOutputB is not null)
        {
            for (int i = GoodOutputB!.Length; i < 4; i++)
            {
                GoodOutputB = GoodOutputB.Append((ArtNetOutputPortStatus2)0).ToArray();
            }
        }
        this.Status3 = status3;
        this.DefaultResponseUID = (defaultResponseUid is not null && defaultResponseUid.Length > 6) ? defaultResponseUid.Take(6).ToArray() : defaultResponseUid;
        this.User = user;
        this.RefreshRate = refreshRate;
        this.BackgroundQueuePolicy = backgroundQueuePolicy;
        this.Filler = (filler is not null && filler.Length > 10) ? filler.Take(10).ToArray() : filler;
    }
    
    // ReSharper disable MemberInitializerValueIgnored
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public ArtPollReplyPacket(BinaryReader reader, int length) : base(reader, length)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
        try
        {
            this.IpAddress = IPAddress.Parse($"{reader.ReadByte()}.{reader.ReadByte()}.{reader.ReadByte()}.{reader.ReadByte()}");
            this.Port = reader.ReadUInt16();
            this.Version = reader.ReadUInt16();
            this.NetSwitch = reader.ReadByte();
            this.SubSwitch = reader.ReadByte();
            this.OemId = reader.ReadUInt16();
            this.UbeaVersion = reader.ReadByte();
            this.Status1 = (Status)reader.ReadByte();
            this.EstaId = reader.ReadUInt16();
            this.PortName = reader.ReadChars(18);
            this.LongName = reader.ReadChars(64);
            this.NodeReport = reader.ReadChars(64);
            reader.ReadByte(); // Skip first byte.
            this.NumberOfPorts = reader.ReadByte();
            this.PortTypes = reader.ReadBytes(4).GetEnumArray<ArtNetPort>();
            this.GoodInput = reader.ReadBytes(4).GetEnumArray<ArtNetInputPortStatus>();
            this.GoodOutput = reader.ReadBytes(4).GetEnumArray<ArtNetOutputPortStatus>();
            this.SwIn = reader.ReadBytes(4);
            this.SwOut = reader.ReadBytes(4);
            this.ACNPriority = reader.ReadByte();
            this.SwMacro = (ArtNetMacro)reader.ReadByte();
            this.SwRemote = (ArtNetRemote)reader.ReadByte();
            this.Spare = reader.ReadBytes(3);
            this.Style = (StyleCode) reader.ReadByte();
            this.Mac = reader.ReadBytes(6);
            // 207 bytes minimum.
            // All requirements are now out of the way.
            this.BindIPAddress = reader.ReadBytes(4);
            this.BindIndex = reader.ReadByte();
            this.Status2 = (Status2) reader.ReadByte();
            this.GoodOutputB = reader.ReadBytes(4).GetEnumArray<ArtNetOutputPortStatus2>();
            this.Status3 = (Status3) reader.ReadByte();
            this.DefaultResponseUID = reader.ReadBytes(6);
            this.User = reader.ReadUInt16();
            this.RefreshRate = reader.ReadUInt16();
            this.BackgroundQueuePolicy = (BackgroundQueuePolicy)reader.ReadByte();
            this.Filler = reader.ReadBytes(10);
        }
        catch (EndOfStreamException)
        {
            // if we hit the end of the stream.
        }
    }

    public override byte[] GetBytes()
    {
        MemoryStream stream = new ();
        StreamWriter writer = new (stream);
        writer.Write(this.ID);
        writer.Write(this.OpCode);
        writer.Write(this.IpAddress.GetAddressBytes());
        writer.Write(this.Port);
        writer.Write(this.Version);
        writer.Write(this.NetSwitch);
        writer.Write(this.SubSwitch);
        writer.Write(this.OemId);
        writer.Write(this.UbeaVersion);
        writer.Write(this.Status1);
        writer.Write(this.EstaId);
        writer.Write(this.PortName);
        writer.Write(this.LongName);
        writer.Write(this.NodeReport);
        writer.Write(0b00000000);
        writer.Write(this.NumberOfPorts);
        writer.Write(this.PortTypes);
        writer.Write(this.GoodInput);
        writer.Write(this.GoodOutput);
        writer.Write(this.SwIn);
        writer.Write(this.SwOut);
        writer.Write(this.ACNPriority);
        writer.Write(this.SwMacro);
        writer.Write(this.SwRemote);
        writer.Write(this.Spare);
        writer.Write(this.Style);
        writer.Write(this.Mac);
        // 207 bytes minimum.
        // All requirements are now out of the way.
        if (this.BindIPAddress is null)
            goto finish;
        writer.Write(this.BindIPAddress);
        if (this.BindIndex is null)
            goto finish;
        writer.Write(this.BindIndex);
        if (this.Status2 is null)
            goto finish;
        writer.Write(this.Status2);
        if (this.GoodOutputB is null)
            goto finish;
        writer.Write(this.GoodOutputB);
        if (this.Status3 is null)
            goto finish;
        writer.Write(this.Status3);
        if (this.DefaultResponseUID is null)
            goto finish;
        writer.Write(this.DefaultResponseUID);
        if (this.User is null)
            goto finish;
        writer.Write(this.User);    
        if (this.RefreshRate is null)
            goto finish;
        writer.Write(this.RefreshRate);
        if (this.BackgroundQueuePolicy is null)
            goto finish;
        writer.Write(this.BackgroundQueuePolicy);
        if (this.Filler is null)
            goto finish;
        writer.Write(this.Filler);
        finish:
        writer.Flush();
        byte[] bytes = stream.ToArray();
        return bytes;
    }
    public override OpCode OpCode => OpCode.OpPollReply;
    public IPAddress IpAddress { get; protected set; }

    public ushort Port { get; protected set; } = 0;

    public ushort Version { get; protected set; } = 0;

    public byte NetSwitch { get; protected set; } = 0;

    public byte SubSwitch { get; protected set; } = 0;

    public ushort OemId { get; protected set; } = 0;

    public byte UbeaVersion { get; protected set; } = 0;

    public Status Status1 { get; protected set; } = 0;

    public ushort EstaId { get; protected set; } = 0;
    // 18
    public char[] PortName { get; protected set; } = Array.Empty<char>();
    // 64
    public char[] LongName { get; protected set; } = Array.Empty<char>();
    // 64
    public char[] NodeReport { get; protected set; } = Array.Empty<char>();
    // Currently the second byte only should be used. Max of 4.
    public ushort NumberOfPorts { get; protected set; } = 0;
    // 4
    public ArtNetPort[] PortTypes { get; protected set; } = new ArtNetPort[4];
    // 4
    public ArtNetInputPortStatus[] GoodInput { get; protected set; } = new ArtNetInputPortStatus[4];
    // 4
    public ArtNetOutputPortStatus[] GoodOutput { get; protected set; } = new ArtNetOutputPortStatus[4];
    // 4
    public byte[] SwIn { get; protected set; } = new byte[4];
    // 4
    public byte[] SwOut { get; protected set; } = new byte[4];

    public byte ACNPriority { get; protected set; } = 0;

    public ArtNetMacro SwMacro { get; protected set; } = 0;

    public ArtNetRemote SwRemote { get; protected set; } = 0;
    // 3 - spare bytes
    public byte[] Spare { get; protected set; } = new byte[3];
    public StyleCode Style { get; protected set; } = 0;
    // 6 - Hi, 1, 2, 3, 4, Lo
    public byte[] Mac { get; protected set; } = new byte[6];
    // 4
    public byte[]? BindIPAddress { get; protected set; }
    public byte? BindIndex { get; protected set; }
    public Status2? Status2 { get; protected set; }
    // 4
    public ArtNetOutputPortStatus2[]? GoodOutputB { get; protected set; }
    
    public Status3? Status3 { get; protected set; }
    // 6
    public byte[]? DefaultResponseUID { get; protected set; }
    public ushort? User { get; protected set; } 
    // DMX Max rate is 44hz, but it is possible to transmit faster rates.
    public ushort? RefreshRate { get; protected set; }
    
    public BackgroundQueuePolicy? BackgroundQueuePolicy { get; protected set; }
    // 10
    public byte[]? Filler { get; protected set; }
    
}