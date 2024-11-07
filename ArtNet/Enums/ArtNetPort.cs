// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         PiZero2W
//    Project:          ArtNet
//    FileName:         ArtNetPort.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/14/2024 12:18
//    Created Date:     10/14/2024 12:18
// -----------------------------------------

namespace ArtNet.Enums;

public enum ArtNetPort : byte
{
    DMX512 =       0b000000,
    Midi =         0b001 << 0,
    Avab =         0b011 << 0,
    ColortranCMX = 0b100 << 0,
    ADB625       = 0b101 << 0,
    ArtNet       = 0b110 << 4,
    DALI         = 0b111 << 5,
    InputData  = 1 << 6,
    OutputData = 1 << 7,
}