// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         PiZero2W
//    Project:          ArtNet
//    FileName:         GenericFlag.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/14/2024 11:16
//    Created Date:     10/14/2024 11:16
// -----------------------------------------

namespace ArtNet.Enums;

[Flags]
public enum GenericFlag : byte
{
    Flag1 = 1 << 0,
    Flag2 = 1 << 1,
    Flag3 = 1 << 2,
    Flag4 = 1 << 3,
    Flag5 = 1 << 4,
    Flag6 = 1 << 5,
    Flag7 = 1 << 6,
    Flag8 = 1 << 7,
}