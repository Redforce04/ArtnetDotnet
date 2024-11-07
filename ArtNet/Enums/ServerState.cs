// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         PiZero2W
//    Project:          ArtNet
//    FileName:         ServerState.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/13/2024 14:32
//    Created Date:     10/13/2024 14:32
// -----------------------------------------

namespace ArtNet.Enums;

/// <summary>
/// Provides state information for what the ArtNet server is currently doing.
/// </summary>
public enum ServerState : byte
{
    /// <summary>
    /// The ArtNet server is starting.
    /// </summary>
    Starting = 0,
    
    /// <summary>
    /// The ArtNet server is waiting for an Ip.
    /// </summary>
    WaitingForIp = 1,
    
    /// <summary>
    /// The ArtNet server is Running.
    /// </summary>
    Running = 2,
    
    /// <summary>
    /// The ArtNet server is Restarting.
    /// </summary>
    Restarting = 3,
    
    /// <summary>
    /// The ArtNet server is Stopping.
    /// </summary>
    Stopping = 4,
    
    /// <summary>
    /// The ArtNet server is Idling.
    /// </summary>
    Idling = 5,
        
    /// <summary>
    /// The ArtNet server is Stopped.
    /// </summary>
    Stopped = 6,
}