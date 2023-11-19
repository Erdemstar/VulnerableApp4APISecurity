namespace VulnerableApp4APISecurity.API.Middlewares;

public class RequestResponseLoggingMiddleware
{
    private readonly ILogger<RequestResponseLoggingMiddleware> _logger;
    private readonly RequestDelegate _next;

    public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        // Log the incoming request
        LogRequest(context.Request);


        // Call the next middleware in the pipeline
        await _next(context);

        // Log the outgoing response
        LogResponse(context.Response);
    }

    private void LogRequest(HttpRequest request)
    {
        _logger.LogInformation($"{Environment.NewLine}--- HTTP REQUEST ---{Environment.NewLine}" +
                               $"Schema:{request.Scheme} {Environment.NewLine}" +
                               $"Method:{request.Method} {Environment.NewLine}" +
                               $"Host: {request.Host} {Environment.NewLine}" +
                               $"Path: {request.Path} {Environment.NewLine}" +
                               $"QueryString: {request.QueryString} {Environment.NewLine}");
    }


    private void LogResponse(HttpResponse response)
    {
        _logger.LogInformation($"{Environment.NewLine} --- HTTP RESPONSE ---{Environment.NewLine}" +
                               $"StatusCode:{response.StatusCode} {Environment.NewLine}" +
                               $"Headers: {response.Headers} {Environment.NewLine}" +
                               $"ContentType: {response.ContentType} {Environment.NewLine}" +
                               $"Cookies: {response.Cookies} {Environment.NewLine}");
    }
}

// Extension method used to add the middleware to the HTTP request pipeline.
public static class RequestResponseLoggingMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestResponseLogging(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestResponseLoggingMiddleware>();
    }
}