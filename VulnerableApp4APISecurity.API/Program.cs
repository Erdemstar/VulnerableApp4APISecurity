using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

using VulnerableApp4APISecurity.Core.Interfaces.Utility.Database;
using VulnerableApp4APISecurity.Core.Interfaces.Utility.JWT;
using VulnerableApp4APISecurity.Infrastructure.Utility.Database;
using VulnerableApp4APISecurity.Infrastructure.Utility.JWT;

using VulnerableApp4APISecurity.Infrastructure.Repositories.Account;
using VulnerableApp4APISecurity.Infrastructure.Repositories.Card;
using VulnerableApp4APISecurity.Infrastructure.Repositories.Profile;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

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
        ValidIssuer = builder.Configuration["JWTSettings:Issuer"],
        ValidAudience = builder.Configuration["JWTSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWTSettings:Key"]))

    };
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "VulnerableApp4APISecurity",
        Description = "This project created for ",
        Contact = new OpenApiContact() { Name = "Erdemstar", Email = "erdem.yildiz@windowslive.com" },
    });
});

//mapper

builder.Services.Configure<JWTSettings>(builder.Configuration.GetSection(nameof(JWTSettings)));
builder.Services.AddSingleton<IJWTSettings>(sp => sp.GetRequiredService<IOptions<JWTSettings>>().Value);


builder.Services.AddSingleton<IDatabaseSettings>(sp => sp.GetRequiredService<IOptions<DatabaseSettings>>().Value);
builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection(nameof(DatabaseSettings)));


//.Services.AddSingleton(mapper);
builder.Services.AddSingleton<JWTAuthManager>();
builder.Services.AddSingleton<AccountRepository>();
builder.Services.AddSingleton<ProfileRepository>();
builder.Services.AddSingleton<CardRepository>();

builder.Services.AddHealthChecks();


var app = builder.Build();

app.UseRouting();

app.UseHealthChecks("/healthz", new HealthCheckOptions
{
    ResponseWriter = async (arg1, arg2) =>
    {
        arg1.Response.ContentType = "text/plain";
        await arg1.Response.WriteAsync("App is working bitch");
    }
});

app.UseAuthentication();

app.UseAuthorization();

//app.UseMiddleware<RequestResponseLoggingMiddleware>();

//app.UseRequestResponseLogging();

app.UseSwagger();

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "VulnerableApp4APISecurity");
});

app.MapControllers();

app.Run();

