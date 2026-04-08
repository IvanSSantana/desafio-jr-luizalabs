namespace GpsPoiApp.Models;

public class Point
{  
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public int X { get; private set; }
    public int Y { get; private set; }

    public Point(int x, int y, string? name = null)
    {
        if (x < 0 || y < 0)
        {
            throw new ArgumentException("Coordinates must be positive.");
        }

        X = x;
        Y = y;
        Name = name;
        Id = Guid.NewGuid();
    }

    public double DistanceMeter(Point otherPoint)
    {
        // Mathematical formula to calculate the distance between two points in a cartesian plane
        double calculatedDistance = Math.Round(Math.Sqrt(Math.Pow(otherPoint.X - X, 2) + Math.Pow(otherPoint.Y - Y, 2)), 2);
        return calculatedDistance;
    }

    public bool IsInRange(Point referencePoint, double maxDistance)
    {
        double calculatedDistance = DistanceMeter(referencePoint);
        return calculatedDistance <= maxDistance;
    }
}