// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         PiZero2W
//    Project:          ArtNet
//    FileName:         ServerStatus.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/13/2024 14:30
//    Created Date:     10/13/2024 14:30
// -----------------------------------------

namespace ArtNet.Enums;

/// <summary>
/// Provides control options for the ArtNet Server.
/// </summary>
public enum ServerActions : byte
{
    /// <summary>
    /// Indicates that the ArtNet server should idle and await for instructions.
    /// </summary>
    Idle = 0,
    
    /// <summary>
    /// Indicates that the ArtNet server should be run.
    /// </summary>
    Run = 1,
    
    /// <summary>
    /// Indicates that the ArtNet server should restart.
    /// </summary>
    Restart = 2,
    
    /// <summary>
    /// Indicates that the ArtNet server should be stopped.
    /// </summary>
    Stop = 3,
}