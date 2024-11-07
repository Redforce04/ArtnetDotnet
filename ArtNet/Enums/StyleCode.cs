// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         PiZero2W
//    Project:          ArtNet
//    FileName:         StyleCode.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/14/2024 14:26
//    Created Date:     10/14/2024 14:26
// -----------------------------------------

namespace ArtNet.Enums;

public enum StyleCode : byte
{
    StNode       = 0x00,
    StController = 0x01,
    StMedia      = 0x02,
    StRoute      = 0x03,
    StBackup     = 0x04,
    StConfig     = 0x05,
    StVisual     = 0x06,
}