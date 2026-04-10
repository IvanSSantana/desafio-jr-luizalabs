using GpsPoiApp.Infrastructure;
using GpsPoiApp.Models;

namespace GpsPoiApp.Repository;

public class PointRepository : IPointRepository
{
    private readonly AppDbContext _dbContext;

    public PointRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Point AddPoint(Point point)
    {
        var addedPoint = _dbContext.Points.Add(point);
        _dbContext.SaveChanges();

        return addedPoint.Entity; 
    }

    public List<Point> GetAllPoints()
    {
        return _dbContext.Points.ToList(); // Returns the entire list as a List<Point>
    }

    public List<Point> GetPointsByProximity(int x, int y, double maxDistance)
    {
        List<Point> dbToList = _dbContext.Points.ToList(); // Linq query to filter points based on proximity

        List<Point> nearbyPoints = dbToList.Where(point => point.IsInRange(new Point(x, y), maxDistance)).ToList();

        return nearbyPoints;
    }
}