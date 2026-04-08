using GpsPoiApp.Infrastructure;
using GpsPoiApp.Models;

namespace GpsPoiApp.Services;

public class PointServices : IPointService
{
    private readonly AppDbContext _dbContext;

    public PointServices(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Point AddPoint(Point point)
    {
        return _dbContext.Points.Add(point).Entity; // Adds the point to the database and returns the result as Entity
    }

    public List<Point> GetAllPoints()
    {
        return _dbContext.Points.ToList(); // Returns the entire list as a List<Point>
    }

    public List<Point> GetPointsByProximity(int x, int y, int maxDistance)
    {
        List<Point> nearbyPoints = _dbContext.Points.Where(
            p => p.IsInRange(new Point(x, y), maxDistance)
            ).ToList(); // Linq query to filter points based on proximity
            // Above i convert the result to List type because Linq only works with IEnumerable

        return nearbyPoints;
    }
}