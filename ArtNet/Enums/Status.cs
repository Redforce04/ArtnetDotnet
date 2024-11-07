// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         PiZero2W
//    Project:          ArtNet
//    FileName:         Status.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/14/2024 13:22
//    Created Date:     10/14/2024 13:22
// -----------------------------------------

namespace ArtNet.Enums;

[Flags]
public enum Status
{
    UbeaPresent =   1 << 0,
    RDMCapable =    1 << 1,
    BootedFromROM = 1 << 2,
    //              1 << 3,
    PortProgrammingAuthorityLocateIdentify = 01 << 4,
    PortProgrammingAuthorityWebConfig      = 10 << 4,
    PortProgrammingAuthorityNotUsed        = 11 << 4,
    IndicatorsLocateIdentify               = 01 << 6,
    IndicatorsMuteMode                     = 10 << 6,
    IndicatorsNormalMode                   = 11 << 6,
    
}