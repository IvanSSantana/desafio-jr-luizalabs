using GpsPoiApp.Models;

namespace GpsPoiApp.Tests;

public class DistanceMeterTests
{
    [Fact]
    public void CalculateDistance_ReturnsCorrectValue()
    {
        // Arrange (test scenario setup)
        Point pointA = new Point(27, 12);
        Point pointB = new Point(20, 10);

        // Act (action to be tested)
        double distance = pointA.DistanceMeter(pointB);

        // Assert (verification of the result)
        Assert.Equal(7, distance);
    }
}
