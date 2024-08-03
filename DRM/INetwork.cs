using Godot;

namespace EIV_Common.DRM;

public interface INetwork
{
    /// <summary>
    /// Getting the MultiplayerkPeer for that DRM.
    /// </summary>
    /// <returns></returns>
    public MultiplayerPeer GetPeer();
}
