// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         PiZero2W
//    Project:          ArtNet
//    FileName:         PortAddressProgrammingMode.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/14/2024 11:31
//    Created Date:     10/14/2024 11:31
// -----------------------------------------

namespace ArtNet.Enums;

public enum PortAddressProgrammingAuthority : byte
{
    Unknown = 0,
    FrontPanel = 1,
    NetworkOrWeb = 2,
    NotUsed = 3,
}