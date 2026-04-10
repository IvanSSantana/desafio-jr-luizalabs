namespace GpsPoiApp.Infrastructure.Middlewares;

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlerMiddleware> _logger;

    public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
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
            _logger.LogError("Exception details: {Exception}", ex.Message);
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    public async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        var response = new
        {
            StatusCode = context.Response.StatusCode,
            Message = "Internal Unknown Error. Please try again later."
            
        };

        _logger.LogError("Exception details: {Exception}", ex.Message);
        _logger.LogError("Stack Trace: {StackTrace}", ex.StackTrace);
        await context.Response.WriteAsJsonAsync(response);
    }
}