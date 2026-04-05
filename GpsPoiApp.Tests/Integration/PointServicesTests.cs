using GpsPoiApp.Models;
using GpsPoiApp.Services;

namespace GpsPoiApp.Tests;

public class PointServicesTests
{
    [Fact]
    public async Task AddPoint_IsWorkingCorrectly()
    {
        // Arrange
        Point pointA = new Point(27, 12);
        PointServices pointServices = new();

        // Act
        Point result = await pointServices.AddPoint(pointA);

        // Assert
        Assert.Equal(pointA, result);
    }

    [Fact]
    public async Task GetAllPoints_ReturnsAllPoints()
    {
        // Arrange
        List<Point> dbMock = new()
        {
            new Point(27, 12, "Cafe"),
            new Point(31, 18, "Gas Station"),
            new Point(15, 12, "Jewelry"),
            new Point(19, 21, "Floriculture"),
            new Point(12, 8, "Pub"),
            new Point(23, 6, "Supermarket"),
            new Point(28, 2, "Steakhouse"),
        };
        PointServices pointServices = new();

        // Act
        List<Point> result = await pointServices.GetAllPoints();

        // Assert
        Assert.NotEmpty(result);

         // I'm using equivalent below, because equal comparers if instance is the same, and in this case, i'm only want to check the contents
        Assert.Equivalent(dbMock, result);
    }

    [Fact]
    public async Task GetPointsByProximity_ReturnsCorrectPoints()
    {
        // Arrange
        PointServices pointServices = new();
        int x = 20, y = 10, maxDistance = 10;
        List<Point> awaitedResponse = new() // According the documentation example
        {
            new Point(27, 12, "Cafe"),
            new Point(15, 12, "Jewelry"),
            new Point(12, 8, "Pub"),
            new Point(23, 6, "Supermarket"),
        };

        // Act
        List<Point> result = await pointServices.GetPointsByProximity(x, y, maxDistance);

        // Assert
        Assert.NotEmpty(result);
        Assert.Equivalent(result, awaitedResponse);
    }
}   