// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         PiZero2W
//    Project:          ArtNet
//    FileName:         ArtNetInputPortStatus.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/14/2024 12:30
//    Created Date:     10/14/2024 12:30
// -----------------------------------------

namespace ArtNet.Enums;

[Flags]
public enum ArtNetInputPortStatus : byte
{
    SACNInput =             1 << 0,
    // Unused =             1 << 1,
    ReceiveErrorsDetected = 1 << 2,
    InputDisabled =         1 << 3,
    DMXTextPackets =        1 << 4,
    DMXSIPs =               1 << 5,
    DMXTestPackets =        1 << 6,
    DataReceived =          1 << 7,
}