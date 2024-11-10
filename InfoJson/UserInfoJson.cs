using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using EIV_Common.Platform;
using EIV_Common.Extensions;

namespace EIV_Common.InfoJson;

public class UserInfoJson
{
    [JsonProperty(ItemConverterType = typeof(StringEnumConverter))]
    public PlatformType Platform { get; set; } = PlatformType.Unknown;
    public string Name { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public object AdditionalData { get; set; } = new();

    public string CreateUserId()
    {
        return $"{UserId}@{Platform.GetEnumMemberValue()}";
    }
}
