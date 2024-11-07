// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         PiZero2W
//    Project:          ArtNet
//    FileName:         ArtNetRemote.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/14/2024 12:45
//    Created Date:     10/14/2024 12:45
// -----------------------------------------

namespace ArtNet.Enums;

[Flags]
public enum ArtNetRemote : byte
{
    Remote1 = 1 << 0,
    Remote2 = 1 << 1,
    Remote3 = 1 << 2,
    Remote4 = 1 << 3,
    Remote5 = 1 << 4,
    Remote6 = 1 << 5,
    Remote7 = 1 << 6,
    Remote8 = 1 << 7,
}