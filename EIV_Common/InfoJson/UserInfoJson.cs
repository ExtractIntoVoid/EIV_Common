﻿using EIV_Common.Extensions;
using EIV_Common.Platform;
using System.Text.Json.Serialization;

namespace EIV_Common.InfoJson;

public class UserInfoJson
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public PlatformType Platform { get; set; } = PlatformType.Unknown;
    public string Name { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string AdditionalData { get; set; } = string.Empty;

    public string CreateUserId()
    {
        return $"{UserId}@{Platform.GetEnumMemberValue()}";
    }
}
