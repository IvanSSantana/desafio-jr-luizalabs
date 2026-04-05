using GpsPoiApp.Models;

namespace GpsPoiApp.Services;

public interface IPointService
{
    Task<Point> AddPoint(Point point);
    Task<List<Point>> GetAllPoints();
    Task<List<Point>> GetPointsByProximity(int x, int y, int maxDistance);
}
