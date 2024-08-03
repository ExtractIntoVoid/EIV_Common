using Godot;

namespace EIV_Common.DRM;

public class UnknownDRM : IDRM
{
    public DRMType DRMType => DRMType.Unknown;

    public Node? GetNode()
    {
        return null;
    }

    public IUser? GetUser()
    {
        return null;
    }

    public INetwork? GetNetwork()
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
