using GpsPoiApp.Models;

namespace GpsPoiApp.Tests;

public class  InstantiatingPointTests
{
    [Fact]
    public void InstantiatingPoint_ReturnsSuccess()
    {
        // Arrange & Act 
        Point pointA = new Point(10, 10);

        // Assert 
        Assert.NotNull(pointA);
    }

    [Fact]
    public void InstantiatingPoint_ReturnsArgumentException()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentException>(() => new Point(-10, -10));
        Assert.Throws<ArgumentException>(() => new Point(-10, 10));
        Assert.Throws<ArgumentException>(() => new Point(10, -10));
    }
}
