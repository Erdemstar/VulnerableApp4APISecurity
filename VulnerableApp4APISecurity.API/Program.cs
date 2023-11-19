using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Prometheus;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;
using VulnerableApp4APISecurity.API.Extension;
using VulnerableApp4APISecurity.Core.Interfaces.Utility.Database;
using VulnerableApp4APISecurity.Core.Interfaces.Utility.JWT;
using VulnerableApp4APISecurity.Core.Mapping;
using VulnerableApp4APISecurity.Infrastructure.Repositories.Account;
using VulnerableApp4APISecurity.Infrastructure.Repositories.Card;
using VulnerableApp4APISecurity.Infrastructure.Repositories.Profile;
using VulnerableApp4APISecurity.Infrastructure.Repositories.RefreshToken;
using VulnerableApp4APISecurity.Infrastructure.Utility.Database;
using VulnerableApp4APISecurity.Infrastructure.Utility.JWT;

var LoggerConfig = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", false, true)
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

    //Serilog
    builder.Host.UseSerilog();

    //Mapper
    builder.Services.AddAutoMapper(typeof(MappingProfile));

    //Extension
    builder.AddAppSwaggerExtension();
    builder.AddAppJWTExtension();

    builder.Services.AddControllers();

    //Custom DI
    builder.Services.Configure<JWTSettings>(builder.Configuration.GetSection(nameof(JWTSettings)));
    builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection(nameof(DatabaseSettings)));
    
    builder.Services.AddSingleton<IJWTSettings>(sp => sp.GetRequiredService<IOptions<JWTSettings>>().Value);
    builder.Services.AddSingleton<IDatabaseSettings>(sp => sp.GetRequiredService<IOptions<DatabaseSettings>>().Value);
    builder.Services.AddSingleton<JWTAuthManager>();
    builder.Services.AddSingleton<AccountRepository>();
    builder.Services.AddSingleton<ProfileRepository>();
    builder.Services.AddSingleton<CardRepository>();
    builder.Services.AddSingleton<RefreshTokenRepository>();

    builder.Services.AddHealthChecks();

    var app = builder.Build();

    //Custom middleware
    //app.UseRequestResponseLogging();

    app.UseRouting();

    app.UseSerilogRequestLogging(options =>
    {
        //Emit debug-level events instead of the defaults
        options.GetLevel = (_, _, _) => LogEventLevel.Debug;

        //Attach additional properties to the request completion event
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

    app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "VulnerableApp4APISecurity"); });

    app.MapControllers();

    //Prometheus Log
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