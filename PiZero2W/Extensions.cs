// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         PiZero2W
//    Project:          PiZero2W
//    FileName:         Extensions.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/12/2024 13:5
//    Created Date:     10/12/2024 13:5
// -----------------------------------------

namespace PiZero2W;

using System.Drawing;

public static class Extensions
{
    /// <summary>
    /// Gets a color from RGB values.
    /// </summary>
    /// <param name="red">The red value of the color. 0 - 255</param>
    /// <param name="green">The green value of the color. 0 - 255</param>
    /// <param name="blue">The blue value of the color. 0 - 255</param>
    /// <returns></returns>
    public static Color FromRGB(byte red, byte green, byte blue) => Color.FromArgb(red, green, blue);

    public static int GetGlobalPixelID(ushort deviceID, ushort localPixelID)
    {
        int total = 0;
        for (ushort i = 0; i < deviceID; i++)
            total += Program.NeopixelArrays[i];
        return total + localPixelID;
    }
}