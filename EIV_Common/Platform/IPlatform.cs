﻿namespace EIV_Common.Platform;

/// <summary>
/// Platform interface
/// </summary>
public interface IPlatform
{
    public PlatformType PlatformType { get; }

    /// <summary>
    /// Called when GameManager starts.
    /// </summary>
    public void Init();

    /// <summary>
    /// Before Quitting the game.
    /// </summary>
    public void ShutDown();

    /// <summary>
    /// Getting a Object to add into the GameManager.
    /// <br/> If null it will not add it
    /// </summary>
    /// <returns>Null or a Object</returns>
    public object? GetObject();


    /// <summary>
    /// Getting the User related information
    /// <br/> If not initialized or something wrong returns null.
    /// </summary>
    /// <returns>User</returns>
    public IUser? GetUser();
}
