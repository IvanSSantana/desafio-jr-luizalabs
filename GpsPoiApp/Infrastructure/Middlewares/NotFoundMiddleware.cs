using GpsPoiApp.Communication.Responses;

namespace GpsPoiApp.Infrastructure.Middlewares;

public class NotFoundMiddleware
{
    private readonly RequestDelegate _next;

    public NotFoundMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        await _next(httpContext);

        if (httpContext.Response.StatusCode == StatusCodes.Status404NotFound && !httpContext.Response.HasStarted)
        {
            httpContext.Response.ContentType = "application/json";
            
            MessageErrorResponse response = new() // According to RFC 7807 - Problem Details for HTTP APIs
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                Title = "Not found.",
                Status = httpContext.Response.StatusCode,
                Message = "The requested resource was not found.",
                TraceId = httpContext.TraceIdentifier
            };

            await httpContext.Response.WriteAsJsonAsync(response);
        }
    }
}