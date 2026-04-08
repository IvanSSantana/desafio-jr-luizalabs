using GpsPoiApp.Models;

namespace GpsPoiApp.Services;

public interface IPointService
{
    Point AddPoint(Point point);
    List<Point> GetAllPoints();
    List<Point> GetPointsByProximity(int x, int y, double maxDistance);
}
