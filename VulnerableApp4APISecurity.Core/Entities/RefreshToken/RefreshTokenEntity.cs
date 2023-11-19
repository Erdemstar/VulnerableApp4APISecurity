using VulnerableApp4APISecurity.Core.Entities.Base;

namespace VulnerableApp4APISecurity.Core.Entities.RefreshToken;

public class RefreshTokenEntity : BaseEntity
{
    public string? email { get; set; }
    public string? refreshToken { get; set; }
}