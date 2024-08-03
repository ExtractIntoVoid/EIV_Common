using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace EIV_Common.DRM;

[JsonConverter(typeof(StringEnumConverter))]
[DefaultValue(Unknown)]
public enum DRMType
{
    [EnumMember(Value = "unknown")]
    Unknown = -1,
    [EnumMember(Value = "nodrm")]
    DRM_FREE = 0,
    [EnumMember(Value = "steam")]
    DRM_STEAM = 1,
    [EnumMember(Value = "epic")]
    DRM_EPIC = 2,
}
