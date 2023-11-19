namespace VulnerableApp4APISecurity.Core.Interfaces.Utility.JWT;

public interface IJWTSettings
{
    string Secret { get; set; }
    string Issuer { get; set; }
    string Audience { get; set; }
    int AccessTokenExpiration { get; set; }
    int RefreshTokenExpiration { get; set; }
}