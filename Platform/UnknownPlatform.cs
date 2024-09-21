using Godot;

namespace EIV_Common.Platform;

public class UnknownPlatform : IPlatform
{
    public PlatformType PlatformType => PlatformType.Unknown;

    public Node? GetNode()
    {
        return null;
    }

    public IUser? GetUser()
    {
        return null;
    }

    public void Init()
    {
        
    }

    public void ShutDown()
    {
        
    }
}
