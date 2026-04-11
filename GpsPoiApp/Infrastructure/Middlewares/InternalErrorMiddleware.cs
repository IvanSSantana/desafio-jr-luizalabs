namespace GpsPoiApp.Infrastructure.Middlewares;

public class InternalErrorMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<InternalErrorMiddleware> _logger;

    public InternalErrorMiddleware(RequestDelegate next, ILogger<InternalErrorMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        { // Try to execute the next middleware in the pipeline
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    public async Task HandleExceptionAsync(HttpContext httpContext, Exception ex)
    {
        httpContext.Response.ContentType = "application/json";
        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

        var response = new // According to RFC 7807 - Problem Details for HTTP APIs
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            Title = "Internal Server Error",
            StatusCode = httpContext.Response.StatusCode,
            Message = "An unexpected error occurred. Please try again later.",
            TraceId = httpContext.TraceIdentifier
        };

        _logger.LogError("An unexpected error occurred: {TraceId}", httpContext.TraceIdentifier);
        _logger.LogError("Exception details: {Exception}", ex.Message);
        _logger.LogError("Stack Trace: {StackTrace}", ex.StackTrace);
        await httpContext.Response.WriteAsJsonAsync(response);
    }
}