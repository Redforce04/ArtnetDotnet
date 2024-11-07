// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         PiZero2W
//    Project:          ArtNet
//    FileName:         Status3.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/14/2024 14:14
//    Created Date:     10/14/2024 14:14
// -----------------------------------------

namespace ArtNet.Enums;

public enum Status3 : byte
{
    BackgroundDiscoverySupported = 1 << 0,
    BackgroundQueueSupported     = 1 << 1,
    RDMNetSupported              = 1 << 2,
    InputOutputSwitchingSupport  = 1 << 3,
    LLRPServed                   = 1 << 4,
    FailoverSupported            = 1 << 5,
    FailsafeStateOutputTo0       = 0b01 << 6,
    FailsafeStateMaxOutput       = 0b10 << 6,
    FailsafeStateScene           = 0b11 << 6,
}