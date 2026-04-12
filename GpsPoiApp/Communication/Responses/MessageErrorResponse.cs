namespace GpsPoiApp.Communication.Responses;

public class MessageErrorResponse
{
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public int Status { get; set; } = default;
    public string Message { get; set; } = string.Empty;
    public string TraceId { get; set; } = string.Empty;
}