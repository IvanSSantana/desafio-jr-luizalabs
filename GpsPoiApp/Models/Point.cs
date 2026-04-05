namespace GpsPoiApp.Models;

public class Point
{  
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
    }

    public int DistanceMeter(Point otherPoint)
    {
        // Mathematical formula to calculate the distance between two points in a cartesian plane
        int calculatedDistance = (int)Math.Round(Math.Sqrt(Math.Pow(otherPoint.X - X, 2) + Math.Pow(otherPoint.Y - Y, 2)), 0);
        return calculatedDistance;
    }

    public bool IsInRange(Point referencePoint, int maxDistance)
    {
        int calculatedDistance = DistanceMeter(referencePoint);
        return calculatedDistance <= maxDistance;
    }
}