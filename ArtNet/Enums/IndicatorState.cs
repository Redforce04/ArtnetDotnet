// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         PiZero2W
//    Project:          ArtNet
//    FileName:         IndicatorState.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/14/2024 12:6
//    Created Date:     10/14/2024 12:6
// -----------------------------------------

namespace ArtNet.Enums;

public enum IndicatorState : byte
{
    Unknown = 0,
    LocateIdentify = 1,
    Mute = 2,
    Normal = 3,
}