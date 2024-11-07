// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         PiZero2W
//    Project:          ArtNet
//    FileName:         OpPollPacket.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/13/2024 18:8
//    Created Date:     10/13/2024 18:8
// -----------------------------------------

namespace ArtNet.Packets;

using Enums;

public class ArtPollPacket : ArtNetPacket
{
    public override OpCode OpCode => OpCode.OpPoll;

    public ArtPollPacket(BinaryReader reader, int length) : base(reader, length)
    {
        this.ProtocolVersion = reader.ReadUInt16();
        this.Flags = (Flags)reader.ReadByte();
        this.DiagnosticPriority = reader.ReadByte();
        try
        {
            this.TargetPortTop = reader.ReadUInt16();
            this.TargetPortBottom = reader.ReadUInt16();
            this.EstaID = reader.ReadUInt16();
            this.OemID = reader.ReadUInt16();
        }
        catch (EndOfStreamException)
        {
            // Used if these items are not implemented in the packet.
        }
    }
    
    /// <summary>
    /// Flags that can be set.
    /// </summary>
    public Flags Flags { get; protected set; }
    
    /// <summary>
    /// The lowest priority of diagnostics message that should be sent.
    /// </summary>
    public ushort DiagnosticPriority { get; protected set; } 

    /// <summary>
    /// Top of the range of Port-Addresses to be tested if Targeted Mode is active.
    /// </summary>
    public ushort? TargetPortTop { get; protected set; }

    /// <summary>
    ///  Bottom of the range of Port-Addresses to be tested if Targeted Mode is active.
    /// </summary>
    public ushort? TargetPortBottom { get; protected set; }

    /// <summary>
    /// The ESTA Manufacturer Code is assigned by ESTA and uniquely identifies the manufacturer that generated this packet.
    /// </summary>
    public ushort? EstaID { get; protected set; }

    // #define OemUnknown 0x00ff //Manufacturer: Artistic Licence Engineering Ltd ProductName: OemUnknown NumDmxIn: 0 NumDmxOut: 0 DmxPortPhysical: n RdmSupported: n SupportEmail:  SupportName:  CoWeb:
    // #define OemGlobal 0xffff //Manufacturer: Artistic Licence Engineering Ltd ProductName: OemGlobal NumDmxIn: 0 NumDmxOut: 0 DmxPortPhysical: n RdmSupported: n SupportEmail:  SupportName:  CoWeb:
    /// <summary>
    /// The Oem code uniquely identifies the product sending this packet.
    /// </summary>
    public ushort? OemID { get; protected set; }
}