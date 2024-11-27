namespace EIV_Common.Platform;

/// <summary>
/// User dependent.
/// </summary>
public interface IUser
{

    /// <summary>
    /// Called for login.
    /// 
    /// Only if <see cref="IPlatform.PlatformType"/> is <see cref="PlatformType.PlatformEpic"/>
    /// </summary>
    /// <param name="loginData">Login Data for Logging in</param>
    public void Login(object loginData);

    /// <summary>
    /// Called when Logged Out.
    /// </summary>
    public void LogOut();

    /// <summary>
    /// Getting User Id
    /// </summary>
    /// <returns>The UserId on that platform as a string</returns>
    public string GetUserId();

    /// <summary>
    /// Getting UserName
    /// </summary>
    /// <returns>The UserName on that platform as a string</returns>
    public string GetUserName();
}
