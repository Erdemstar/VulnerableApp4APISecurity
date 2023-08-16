using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using VulnerableApp4APISecurity.Core.Entities.Account;
using VulnerableApp4APISecurity.Core.Interfaces.Utility.JWT;

namespace VulnerableApp4APISecurity.Infrastructure.Utility.JWT
{
	public class JWTAuthManager
	{
        private readonly IJWTSettings _settings;

        public JWTAuthManager(IJWTSettings settings)
        {
            _settings = settings;
        }

        public string GenerateTokens(AccountEntity account)
        {

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.ASCII.GetBytes(_settings.Key);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[] {

                    new Claim(ClaimTypes.NameIdentifier, account.Id),
                    new Claim(ClaimTypes.Email, account.Email),
                    new Claim(ClaimTypes.Role, account.Role),
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
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
                return null;
            }

        }
    }
}

