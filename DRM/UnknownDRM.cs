using Godot;

namespace EIV_Common.DRM;

public class UnknownDRM : IDRM
{
    public DRMType DRMType => DRMType.Unknown;

    public Node? GetNode()
    {
        return null;
    }

    public IUser GetUser()
    {
        return new UnknownUser();
    }
    public void Init()
    {
        
    }

    public void ShutDown()
    {
        
    }
}

public class UnknownUser : IUser
{
    public string GetUserID()
    {
        return string.Empty;
    }

    public string GetUserName()
    {
        return string.Empty;
    }
}
