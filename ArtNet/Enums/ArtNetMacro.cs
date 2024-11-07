// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         PiZero2W
//    Project:          ArtNet
//    FileName:         ArtNetMacro.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/14/2024 12:43
//    Created Date:     10/14/2024 12:43
// -----------------------------------------

namespace ArtNet.Enums;

[Flags]
public enum ArtNetMacro : byte
{
    Macro1 = 1 << 0,
    Macro2 = 1 << 1,
    Macro3 = 1 << 2,
    Macro4 = 1 << 3,
    Macro5 = 1 << 4,
    Macro6 = 1 << 5,
    Macro7 = 1 << 6,
    Macro8 = 1 << 7,
    
}