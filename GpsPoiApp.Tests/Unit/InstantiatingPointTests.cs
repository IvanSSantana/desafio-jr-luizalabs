using GpsPoiApp.Models;

namespace GpsPoiApp.Tests;

public class  InstantiatingPointTests
{
    [Fact]
    public void InstantiatingPoint_ShouldReturnSuccess()
    {
        // Arrange & Act 
        Point pointA = new Point(10, 10);

        // Assert 
        Assert.NotNull(pointA);
    }

    [Fact]
    public void InstantiatingPoint_ShouldReturnArgumentExceptionWhenCoordinatesAreNegative()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => new Point(-10, -10));
        Assert.Throws<ArgumentOutOfRangeException>(() => new Point(-10, 10));
        Assert.Throws<ArgumentOutOfRangeException>(() => new Point(10, -10));
    }
}
