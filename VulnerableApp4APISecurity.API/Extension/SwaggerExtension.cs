using Microsoft.OpenApi.Models;

namespace VulnerableApp4APISecurity.API.Extension;

public static class SwaggerExtension
{
    public static WebApplicationBuilder AddAppSwaggerExtension(this WebApplicationBuilder builder)
    {
        // Swagger Settings with Authorize Parameters
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll",
                corsSetting => { corsSetting.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); });
        });
        builder.Services.AddSwaggerGen(options =>
        {
            options.EnableAnnotations();
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "VulnerableApp4APISecurity",
                Description =
                    "This repository was developed using .NET 7.0 API technology based on findings listed in the OWASP 2019 API Security Top 10. ",
                Contact = new OpenApiContact { Name = "Erdemstar", Email = "erdem.yildiz@windowslive.com" }
            });
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "Enter the Bearer Authorization string as following `Bearer Generated JWT Token`",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer"
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        In = ParameterLocation.Header
                    },
                    Array.Empty<string>()
                }
            });
        });

        return builder;
    }
}