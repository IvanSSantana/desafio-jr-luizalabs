using GpsPoiApp.Models;

namespace GpsPoiApp.Infrastructure;

public static class DbSeeding
{
    public static void Seed(AppDbContext context)
    {
        if (context.Points.Any()) return; // if data already exists don't seed
        
        List<Point> seed = new()
        {
            new Point(27, 12, "Cafe"),
            new Point(31, 18, "Gas Station"),
            new Point(15, 12, "Jewelry"),
            new Point(19, 21, "Floriculture"),
            new Point(12, 8, "Pub"),
            new Point(23, 6, "Supermarket"),
            new Point(28, 2, "Steakhouse"),
        };

        context.Points.AddRange(seed);
        context.SaveChanges();
        
    }
}