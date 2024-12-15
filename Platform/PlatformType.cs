using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace EIV_Common.Platform;

[JsonConverter(typeof(JsonStringEnumConverter))]
[DefaultValue(Unknown)]
public enum PlatformType
{
    [EnumMember(Value = "unknown")]
    Unknown = -1,
    [EnumMember(Value = "free")]
    PlatformFree,
    [EnumMember(Value = "steam")]
    PlatformSteam,
    [EnumMember(Value = "epic")]
    PlatformEpic,
    [EnumMember(Value = "itch")]
    PlatformItch,
}
