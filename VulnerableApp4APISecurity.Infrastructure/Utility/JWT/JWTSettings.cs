using System.Text.Json.Serialization;
using VulnerableApp4APISecurity.Core.Interfaces.Utility.JWT;

namespace VulnerableApp4APISecurity.Infrastructure.Utility.JWT;

public class JWTSettings : IJWTSettings
{
    [JsonPropertyName("secret")] public string? Secret { get; set; }

    [JsonPropertyName("issuer")] public string? Issuer { get; set; }

    [JsonPropertyName("audience")] public string? Audience { get; set; }

    [JsonPropertyName("accessTokenExpiration")]
    public int AccessTokenExpiration { get; set; }

    [JsonPropertyName("refreshTokenExpiration")]
    public int RefreshTokenExpiration { get; set; }
}