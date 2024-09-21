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
    Platform_FREE = 0,
    [EnumMember(Value = "steam")]
    Platform_STEAM = 1,
    [EnumMember(Value = "epic")]
    Platform_EPIC = 2,
}
