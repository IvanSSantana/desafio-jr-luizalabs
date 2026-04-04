using GpsPoiApp.Models;

namespace GpsPoiApp.Tests;

public class ProximityCalculatorTests
{
    [Fact]
    public void CalculateDistanceBetweenPoints_ReturnsSuccess()
    {
        // Arrange
        Point pointA = new Point(27, 12);
        Point pointB = new Point(20, 10);
        int maxDistance = 10;

        // Act
        int calculatedDistance = pointA.DistanceMeter(pointB);
        bool isWithinProximity = calculatedDistance <= maxDistance;

        // Assert
        Assert.True(isWithinProximity);        
    }
}
