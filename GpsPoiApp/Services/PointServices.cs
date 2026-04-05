using GpsPoiApp.Models;

namespace GpsPoiApp.Services;

public class PointServices : IPointService
{
    private readonly List<Point> _dbContextMock;
    
    public PointServices()
    { // Mocking database according tasks' documentation example
        _dbContextMock = new List<Point>()
        {
            new Point(27, 12, "Cafe"),
            new Point(31, 18, "Gas Station"),
            new Point(15, 12, "Jewelry"),
            new Point(19, 21, "Floriculture"),
            new Point(12, 8, "Pub"),
            new Point(23, 6, "Supermarket"),
            new Point(28, 2, "Steakhouse"),
        };
    }   

    public Task<Point> AddPoint(Point point)
    {
        _dbContextMock.Add(point);
        return Task.FromResult(point);
    }

    public Task<List<Point>> GetAllPoints()
    {
        return Task.FromResult(_dbContextMock); // Returns the entire list as a List<Point>
    }

    public Task<List<Point>> GetPointsByProximity(int x, int y, int maxDistance)
    {
        List<Point> nearbyPoints = _dbContextMock.Where(
            p => p.IsInRange(new Point(x, y), maxDistance)
            ).ToList(); // Linq query to filter points based on proximity
            // Above i convert the result to List type because Linq only works with IEnumerable

        return Task.FromResult(nearbyPoints);
    }
}