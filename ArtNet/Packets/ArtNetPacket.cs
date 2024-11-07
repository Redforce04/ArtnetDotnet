// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         PiZero2W
//    Project:          ArtNet
//    FileName:         ArtNetPacket.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/13/2024 16:10
//    Created Date:     10/13/2024 16:10
// -----------------------------------------

namespace ArtNet.Packets;

using Enums;

public abstract class ArtNetPacket
{
    /// <summary>
    /// Initializes a new ArtNetPacket. ID & OpCode is skipped by default.
    /// </summary>
    /// <param name="reader"></param>
    protected ArtNetPacket(BinaryReader reader, int length)
    {
    }
    

    protected ArtNetPacket()
    {
    }

    public virtual byte[] GetBytes()
    {
        return Array.Empty<byte>();
    }
        
    /*
     * Big Endian Hi-Lo
     * Little Endian Lo-Hi
     */
    public static ArtNetPacket? DecodePacket(byte[] buffer)
    {
        using Stream stream = new MemoryStream(buffer);
        BinaryReader reader = new (stream);
        try
        {
            int length = buffer.Length - 10;
            if (!reader.ReadBytes(8).SequenceEqual(ArtnetID))
                return null;
            OpCode opcode = (OpCode)reader.ReadUInt16();
            switch (opcode)
            {
                case OpCode.OpPoll:
                        var poll = new ArtPollPacket(reader, length);
                        return poll;
                case OpCode.OpPollReply:
                    return new ArtPollReplyPacket(reader, length);
                case OpCode.OpOutputDmx:
                    return new OutputDMXPacket(reader, length);
                case OpCode.OpNzs:
                    break;
                case OpCode.OpSync:
                    break;
                case OpCode.OpAddress:
                    break;
                case OpCode.OpInput:
                    break;
                case OpCode.OpTodRequest:
                    break;
                case OpCode.OpTodData:
                    break;
                case OpCode.OpTodControl:
                    break;
                case OpCode.OpRdm:
                    break;
                case OpCode.OpRdmSub:
                    break;
                default:
                    return null;
            }
        }
        catch (Exception e)
        {
            Log.Exception(e);
        }
        return null;
    }

    protected virtual void Decode(BinaryReader reader)
    {
        
    }

    public static byte[] ArtnetID =
    {
        0x41, 0x72, 0x74, 0x2d, 0x4e, 0x65, 0x74, 0x00,
    }; 

    public byte[] ID => ArtnetID;

    public abstract OpCode OpCode { get; }

    // hi-lo
    public ushort ProtocolVersion { get; protected set; }
}