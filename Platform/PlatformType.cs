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
    PlatformFree = 0,
    [EnumMember(Value = "steam")]
    PlatformSteam = 1,
    [EnumMember(Value = "epic")]
    PlatformEpic = 2,
}
