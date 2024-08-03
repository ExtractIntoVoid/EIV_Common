
namespace EIV_Common.DRM;

/// <summary>
/// User dependent.
/// </summary>
public interface IUser
{
    /// <summary>
    /// Getting User Id
    /// </summary>
    /// <returns>The User Id on that platform as a string</returns>
    public string GetUserID();

    /// <summary>
    /// Getting User Name
    /// </summary>
    /// <returns>The User Name on that platform as a string</returns>
    public string GetUserName();
}
