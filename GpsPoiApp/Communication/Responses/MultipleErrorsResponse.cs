namespace GpsPoiApp.Communication.Responses;

public class MultipleErrorsResponse
{
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public int Status { get; set; } = default;
    public Dictionary<string, string[]> Errors { get; set; } = default!;
    public string TraceId { get; set; } = string.Empty;
}