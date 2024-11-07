// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         PiZero2W
//    Project:          PiZero2W
//    FileName:         AddressData.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/12/2024 14:27
//    Created Date:     10/12/2024 14:27
// -----------------------------------------

namespace ArtNet.Structs;

public struct DMXAddressData(DMXAddress address, byte data)
{
    public readonly DMXAddress Address = address;
    public readonly byte Data = data;
}