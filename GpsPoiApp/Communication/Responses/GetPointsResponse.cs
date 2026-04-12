namespace GpsPoiApp.Communication.Responses;

public class GetPointsResponse
{
    public List<CreatedPointResponse> Points { get; set; } = new();
}