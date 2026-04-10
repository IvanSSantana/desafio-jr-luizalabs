using GpsPoiApp.Models;

namespace GpsPoiApp.Communication.Responses;

public class GetPointsResponse
{
    public List<Point> Points { get; set; } = new();
}