namespace GpsPoiApp.Communication.Responses;

public class GetPointsResponse
{
    public List<SimplifiedPointResponse> Points { get; set; } = new();
}