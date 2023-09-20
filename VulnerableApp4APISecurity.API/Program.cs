using System.Text;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Authentication.JwtBearer;

using VulnerableApp4APISecurity.Core.Mapping;
using VulnerableApp4APISecurity.Core.Interfaces.Utility.Database;
using VulnerableApp4APISecurity.Core.Interfaces.Utility.JWT;

using VulnerableApp4APISecurity.Infrastructure.Utility.Database;
using VulnerableApp4APISecurity.Infrastructure.Utility.JWT;
using VulnerableApp4APISecurity.Infrastructure.Repositories.Account;
using VulnerableApp4APISecurity.Infrastructure.Repositories.Card;
using VulnerableApp4APISecurity.Infrastructure.Repositories.Profile;

using Serilog;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;

using Prometheus;

var LoggerConfig = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithProcessName()
    .Enrich.WithProcessId()
    .Enrich.WithThreadName()
    .Enrich.WithThreadId()
    .Enrich.WithEnvironmentUserName()
    .Enrich.WithEnvironmentName()
    .Enrich.WithMemoryUsage()
    .WriteTo.Debug()
    .WriteTo.Console()
    .WriteTo.File("./Log/Serilog.txt")
    .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://elasticsearch:9200"))
    {
        AutoRegisterTemplate = true,
        TemplateName = "VulnerableApp4APISecurity API Log",
        IndexFormat = "vulnerableapp4apisecurity-log-{0:yyyy.MM.dd}"
    })
    .Enrich.FromLogContext()
    .ReadFrom.Configuration(LoggerConfig)
    .CreateLogger();

try
{

    var builder = WebApplication.CreateBuilder(args);

    // Serilog
    builder.Host.UseSerilog();

    //Mapper
    builder.Services.AddAutoMapper(typeof(MappingProfile));

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
            Description = "This repository was developed using .NET 7.0 API technology based on findings listed in the OWASP 2019 API Security Top 10. ",
            Contact = new OpenApiContact() { Name = "Erdemstar", Email = "erdem.yildiz@windowslive.com" },
        });
    });


    builder.Services.Configure<JWTSettings>(builder.Configuration.GetSection(nameof(JWTSettings)));
    builder.Services.AddSingleton<IJWTSettings>(sp => sp.GetRequiredService<IOptions<JWTSettings>>().Value);

    builder.Services.AddSingleton<IDatabaseSettings>(sp => sp.GetRequiredService<IOptions<DatabaseSettings>>().Value);
    builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection(nameof(DatabaseSettings)));


    builder.Services.AddSingleton<JWTAuthManager>();
    builder.Services.AddSingleton<AccountRepository>();
    builder.Services.AddSingleton<ProfileRepository>();
    builder.Services.AddSingleton<CardRepository>();

    builder.Services.AddHealthChecks();

    var app = builder.Build();

    //Custom middleware
    //app.UseRequestResponseLogging();

    app.UseRouting();

    app.UseSerilogRequestLogging(options =>
    {
        // // Emit debug-level events instead of the defaults
        options.GetLevel = (_, _, _) => LogEventLevel.Debug;

        // Attach additional properties to the request completion event
        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
            diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
            diagnosticContext.Set("QueryString", httpContext.Request.QueryString);
        };

        options.MessageTemplate =
            "{RequestMethod} {RequestPath}{QueryString} responded {StatusCode} in {Elapsed:0.0000} ms";
    });

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

    app.UseSwagger();

    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "VulnerableApp4APISecurity");
    });

    app.MapControllers();

    // Promethous Log
    app.MapMetrics();

    app.Run();

}
catch (Exception ex)
{
    Log.Fatal(ex, "Unhandled exception");
}
finally
{
    Log.Information("Shut down complete");
    Log.CloseAndFlush();
}
