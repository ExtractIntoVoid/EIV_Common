
namespace EIV_Common.DRM;

/// <summary>
/// User dependent.
/// </summary>
public interface IUser
{

    /// <summary>
    /// Called for login.
    /// 
    /// Only if <see cref="IDRM.DRMType"/> is <see cref="DRMType.DRM_EPIC"/>
    /// </summary>
    /// <param name="loginData">Login Data for Loggning in</param>
    public void Login(object loginData);

    /// <summary>
    /// Called when Logged Out.
    /// </summary>
    public void LogOut();

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
