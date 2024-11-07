// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         PiZero2W
//    Project:          ArtNet
//    FileName:         Status2.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/14/2024 12:52
//    Created Date:     10/14/2024 12:52
// -----------------------------------------

namespace ArtNet.Enums;

[Flags]
public enum Status2 : byte
{
    WebConfigSupport =     1 << 0,
    DHCPConfigured =       1 << 1,
    DHCPCapable =          1 << 2,
    FifteenBitSupport =    1 << 3,
    ArtNetAndSACNSupport = 1 << 4,
    Squaking             = 1 << 5,
    OutputStyleSwitching = 1 << 6,
    RDMSupport =           1 << 7,
}