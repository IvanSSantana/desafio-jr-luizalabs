using GpsPoiApp.Models;

namespace GpsPoiApp.Tests;

public class ProximityCalculatorTests
{
    [Fact]
    public void IsInRange_ShouldReturnTrue()
    {
        // Arrange
        Point pointA = new Point(27, 12);
        Point pointB = new Point(20, 10);
        int maxDistance = 10;

        // Act
        bool isInRange = pointA.IsInRange(pointB, maxDistance);

        // Assert
        Assert.True(isInRange);
    }
    [Fact]
    public void IsInRange_ShouldReturnFalse()
    {
        // Arrange
        Point pointA = new Point(27, 12);
        Point pointB = new Point(20, 10);
        int maxDistance = 5;

        // Act
        bool isInRange = pointA.IsInRange(pointB, maxDistance);

        // Assert
        Assert.False(isInRange);
    }
}
