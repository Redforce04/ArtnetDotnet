// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         PiZero2W
//    Project:          ArtNet
//    FileName:         ArtNetOutputPortStatus2.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/14/2024 12:58
//    Created Date:     10/14/2024 12:58
// -----------------------------------------

namespace ArtNet.Enums;

[Flags]
public enum ArtNetOutputPortStatus2 : byte
{
    // Unused = 0b00000001,
    // Unused = 0b00000010,
    // Unused = 0b00000100,
    // Unused = 0b00001000,
    BackgroundDiscoveryDisabled = 1 << 4,
    DiscoveryNotRunning =         1 << 5,
    OutputStyleContinuous =       1 << 6,
    RDMDisabled =                 1 << 7,
}