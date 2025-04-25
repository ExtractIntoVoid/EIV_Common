using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace EIV_Common.Platform;

[JsonConverter(typeof(JsonStringEnumConverter))]
[DefaultValue(Unknown)]
public enum PlatformType
{
    /// <summary>
    /// Unkown Platform Type, cannot be determined.
    /// </summary>
    [EnumMember(Value = "unknown")]
    Unknown = -1,
    /// <summary>
    /// Free platform solution. (Lan)
    /// </summary>
    [EnumMember(Value = "free")]
    PlatformFree,
    /// <summary>
    /// Steam platform solution.
    /// </summary>
    [EnumMember(Value = "steam")]
    PlatformSteam,
    /// <summary>
    /// Epic Games platform solution.
    /// </summary>
    [EnumMember(Value = "epic")]
    PlatformEpic,
    /// <summary>
    /// Itch platform solution.
    /// </summary>
    [EnumMember(Value = "itch")]
    PlatformItch,
    /// <summary>
    /// Custom platform solution.
    /// </summary>
    [EnumMember(Value = "custom")]
    PlatformCustom,
}
