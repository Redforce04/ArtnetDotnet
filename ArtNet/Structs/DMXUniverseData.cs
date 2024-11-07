// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         PiZero2W
//    Project:          ArtNet
//    FileName:         DMXUniverseData.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/19/2024 12:56
//    Created Date:     10/19/2024 12:56
// -----------------------------------------

namespace ArtNet.Structs;

public class DMXUniverseData
{
    public DMXUniverseData(ArtNetUniverse universe, Dictionary<ushort, DMXAddressData> data)
    {
        this.Universe = universe;
        this.Data = data;
    }
    
    public DMXUniverseData(ArtNetUniverse universe, byte[] data)
    {
        this.Universe = universe;
        Dictionary<ushort, DMXAddressData> conjoinedData = new();
        for (ushort i = 0; i < data.Length; i++)
        {
            byte addr = data[i];
            conjoinedData.Add(i, new DMXAddressData(new DMXAddress(universe.Universe, (byte)(i + 1)), addr));
        }
        this.Data = conjoinedData;
    }
    public ArtNetUniverse Universe { get; }
    public IReadOnlyDictionary<ushort, DMXAddressData> Data { get; }
}