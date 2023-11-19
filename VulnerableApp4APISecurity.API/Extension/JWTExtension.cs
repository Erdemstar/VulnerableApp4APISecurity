using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using VulnerableApp4APISecurity.Infrastructure.Utility.JWT;

namespace VulnerableApp4APISecurity.API.Extension;

public static class JWTExtension
{
    public static WebApplicationBuilder AddAppJWTExtension(this WebApplicationBuilder builder)
    {
        var jwtTokenConfig = builder.Configuration.GetSection("JWTSettings").Get<JWTSettings>();
        builder.Services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(x =>
        {
            x.RequireHttpsMetadata = false;
            x.SaveToken = true;
            x.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtTokenConfig.Issuer,
                ValidAudience = jwtTokenConfig.Audience,
                IssuerSigningKey =
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtTokenConfig.Secret))
            };
        });

        builder.Services.AddScoped<JWTAuthManager>();

        return builder;
    }
}