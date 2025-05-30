﻿namespace EIV_Common.Platform;

public class UnknownPlatform : IPlatform
{
    public PlatformType PlatformType => PlatformType.Unknown;

    public object? GetObject() => null;

    public IUser? GetUser() => null;

    public void Init() { }

    public void ShutDown() { }
}
