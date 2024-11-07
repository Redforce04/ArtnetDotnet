// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         PiZero2W
//    Project:          ArtNet
//    FileName:         OutgoingPacket.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/19/2024 18:40
//    Created Date:     10/19/2024 18:40
// -----------------------------------------

namespace ArtNet.Structs;

using System.Net;
using Packets;

public struct OutgoingPacket(ArtNetPacket packet, EndPoint endPoint)
{
    public ArtNetPacket Packet { get; set; } = packet;

    public EndPoint Endpoint { get; set; } = endPoint;
}