// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         PiZero2W
//    Project:          PiZero2W
//    FileName:         Address.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/12/2024 14:34
//    Created Date:     10/12/2024 14:33
// -----------------------------------------

namespace ArtNet.Structs;

public struct DMXAddress
{
    public DMXAddress(uint fullAddress)
    {
        this.FullAddress = fullAddress;
        var x = Math.DivRem((int)fullAddress, 512);
        this.Universe = (ushort)x.Quotient;
        this.Address = (ushort)x.Remainder;
    }
    public DMXAddress(ushort universe, ushort address)
    {
        this.Universe = universe;
        this.Address = address;
        this.FullAddress = (uint)(universe * 512) + address;
    }

    public override bool Equals(object? obj)
    {
        return false;
    }

    public bool Equals(uint other)
    {
        return other == this.FullAddress;
    }
    
    public bool Equals(DMXAddress other)
    {
        return this.FullAddress == other.FullAddress;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(this.FullAddress);
    }

    public ArtNetUniverse ArtNetUniverse => new ArtNetUniverse(this.Universe);
    public readonly ushort Universe;
    public readonly ushort Address;
    public readonly uint FullAddress;

}