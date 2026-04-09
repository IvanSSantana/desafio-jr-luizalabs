using GpsPoiApp.Models;

namespace GpsPoiApp.Repository;

public interface IPointRepository
{
    Point AddPoint(Point point);
    List<Point> GetAllPoints();
    List<Point> GetPointsByProximity(int x, int y, double maxDistance);
}
