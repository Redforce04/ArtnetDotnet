// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         PiZero2W
//    Project:          ArtNet
//    FileName:         Flags.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/13/2024 17:41
//    Created Date:     10/13/2024 17:41
// -----------------------------------------

namespace ArtNet.Enums;

[Flags]
public enum Flags : byte
{
    None =                      1 << 0,
    Deprecated =                1 << 1,
    DiagnosticMessages =        1 << 2,
    DiagnosticMessagesUnicast = 1 << 3,
    DisableVLCTransmission =    1 << 4,
    EnableTargettedMode =       1 << 5,
}