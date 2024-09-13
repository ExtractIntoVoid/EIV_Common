using Godot;

namespace EIV_Common.DRM;

/// <summary>
/// DRM interface
/// </summary>
public interface IDRM
{
    public DRMType DRMType { get; }

    /// <summary>
    /// Called when GameManager starts.
    /// </summary>
    public void Init();

    /// <summary>
    /// Before Quitting the game.
    /// </summary>
    public void ShutDown();

    /// <summary>
    /// Getting a Node to add into the GameManager.
    /// <br/> If null it will not adding it
    /// </summary>
    /// <returns>Null or a Node</returns>
    public Node? GetNode();


    /// <summary>
    /// Getting the User related informations
    /// <br/> If not initialized or something wrong returns null.
    /// </summary>
    /// <returns>User</returns>
    public IUser? GetUser();
}
