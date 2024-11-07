// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         PiZero2W
//    Project:          ArtNet
//    FileName:         BackgroundQueuePolicy.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/19/2024 18:3
//    Created Date:     10/19/2024 18:3
// -----------------------------------------

namespace ArtNet.Enums;

public enum BackgroundQueuePolicy : byte
{
    StatusNone = 0,
    StatusAdvisory = 1,
    StatusWarning = 2,
    StatusError = 3,
    CollectionDisabled = 4,
}