using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using EIV_Common.DRM;

namespace EIV_Common.InfoJSON;

public class UserInfoJSON
{
    [JsonProperty(ItemConverterType = typeof(StringEnumConverter))]
    public DRMType DRM { get; set; } = DRMType.Unknown;
    public string Name { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public object AdditionalData { get; set; } = new();

    public string CreateUserId()
    {
        return $"{UserId}@{Exts.GetEnumMemberValue(DRM)}";
    }
}
