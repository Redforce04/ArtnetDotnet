// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         PiZero2W
//    Project:          PiZero2W
//    FileName:         PixelTools.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/12/2024 12:23
//    Created Date:     10/12/2024 12:23
// -----------------------------------------

namespace PiZero2W;

using System.Drawing;

public class PixelTools
{
    private static Program _prgm => Program.Instance;
    
    /// <summary>
    /// Sets the color of all pixels defined.
    /// </summary>
    /// <param name="color">The new color.</param>
    public static void SetColorAll(Color color)
    {
        // set all to black.
        for (int i = 0; i < Program.NeopixelCount; i++)
        {
            _prgm.Image.SetPixel(i, 0, color);
        }
        _prgm.NeoPixels.Update();
    }

    /// <summary>
    /// Sets the color of all pixels defined.
    /// </summary>
    /// <param name="Red">The color's red value. 0-255.</param>
    /// <param name="Green">The color's blue value. 0-255.</param>
    /// <param name="Blue">The color's green value. 0-255.</param>
    public static void SetColorAll(byte Red, byte Green, byte Blue) => SetColorAll(Color.FromArgb(Red, Green, Blue));

    /// <summary>
    /// Cascades a color from the first pixel to the last pixel with a pre-specified delay.
    /// </summary>
    /// <param name="color">The color to cascade.</param>
    /// <param name="delayInMS">The delay in Milliseconds.</param>
    public static void CascadeColor(Color color, int delayInMS)
    {
        for (int i = 0; i < Program.NeopixelCount; i++)
        {
            _prgm.Image.SetPixel(i, 0, color);
            _prgm.NeoPixels.Update();
            Thread.Sleep(delayInMS);
        }
    }
    
    /// <summary>
    /// Sets the color of a single pixel.
    /// </summary>
    /// <param name="pixelNumber">The global pixel number.</param>
    /// <param name="color">The color to set the pixel to.</param>
    public static void SetPixelColor(int pixelNumber, Color color)
    {
        _prgm.Image.SetPixel(pixelNumber, 0, color);
        /*for (int i = 0; i < pixelNumber; i++)
        {
            
        }*/
    }

    /// <summary>
    /// Sets the color of a single pixel.
    /// </summary>
    /// <param name="neopixelDevice">The Neopixel device get the pixel.</param>
    /// <param name="pixelNumber">The pixel on the device to change.</param>
    /// <param name="color">The color to set the pixel to.</param>
    public static void SetPixelColor(ushort neopixelDevice, ushort pixelNumber, Color color) => SetPixelColor(Extensions.GetGlobalPixelID(neopixelDevice, pixelNumber), color);

}