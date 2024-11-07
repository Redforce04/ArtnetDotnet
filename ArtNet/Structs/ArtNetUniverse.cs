// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         PiZero2W
//    Project:          ArtNet
//    FileName:         ArtNetUniverse.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/14/2024 9:27
//    Created Date:     10/14/2024 9:27
// -----------------------------------------

namespace ArtNet.Structs;

public record struct ArtNetUniverse
{
    public ArtNetUniverse(byte subnet, byte universe)
    {
        this.Subnet = subnet;
        this.Universe = universe;
    }

    public ArtNetUniverse(ushort fullUniverse)
    {
        this.FullUniverse = fullUniverse;
    }

    /// <summary>
    /// The classic "Working" universe.
    /// </summary>
    public ushort FullUniverse { get; set; }
    
    /// <summary>
    /// The 
    /// </summary>
    public byte Universe 
    {
        get => (byte)Math.DivRem((ushort)(this.FullUniverse - 1), (ushort)16).Remainder;
        set => this.FullUniverse = (ushort)((this.Subnet * 16) + (value + 1));
    }

    public byte Subnet
    {
        get => (byte)Math.DivRem((ushort)(this.FullUniverse - 1), (ushort)16).Quotient;
        set => this.FullUniverse = (ushort)((value * 16) + (this.Universe + 1));
    }
}