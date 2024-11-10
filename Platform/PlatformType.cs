using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace EIV_Common.Platform;

[JsonConverter(typeof(StringEnumConverter))]
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
