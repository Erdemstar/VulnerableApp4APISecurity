using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using VulnerableApp4APISecurity.Core.DTO.Others.Response.Token;
using VulnerableApp4APISecurity.Core.Entities.Account;
using VulnerableApp4APISecurity.Core.Interfaces.Utility.JWT;

namespace VulnerableApp4APISecurity.Infrastructure.Utility.JWT;

public class JWTAuthManager
{
    private readonly IJWTSettings _settings;

    public JWTAuthManager(IJWTSettings settings)
    {
        _settings = settings;
    }

    public TokenResponse GenerateToken(AccountEntity account)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenKey = Encoding.ASCII.GetBytes(_settings.Secret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new(ClaimTypes.NameIdentifier, account.Id),
                new(ClaimTypes.Email, account.Email),
                new(ClaimTypes.Role, account.Role)
            }),
            Expires = DateTime.UtcNow.AddHours(_settings.AccessTokenExpiration),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        var accessToken = tokenHandler.WriteToken(token);
        var refreshToken = GenerateRefreshToken();
        return new TokenResponse
        {
            accessToken = accessToken,
            refreshToken = refreshToken
        };
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }

    public string TakeEmailFromJWT(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var JWTClaims = handler.ReadJwtToken(token).Claims;
            var email = JWTClaims.FirstOrDefault(c => c.Type == "email").Value;

            return email;
        }
        catch
        {
            return null;
        }
    }

    public string TakeUserIdFromJWT(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var JWTClaims = handler.ReadJwtToken(token).Claims;
            var email = JWTClaims.FirstOrDefault(c => c.Type == "nameid").Value;

            return email;
        }
        catch
        {
            return "";
        }
    }
}