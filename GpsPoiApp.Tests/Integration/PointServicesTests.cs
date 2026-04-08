using FluentAssertions;
using GpsPoiApp.Helper;
using GpsPoiApp.Infrastructure;
using GpsPoiApp.Models;
using GpsPoiApp.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace GpsPoiApp.Tests;

public class PointServicesTests : IDisposable
{
    private readonly AppDbContext _dbContext;
    private readonly SqliteConnection _dbConnection;

    // Setup method to create an in-memory database context for every testing
    public PointServicesTests()
    {
        var (dbContext, connection) = TestDbFactory.CreateInMemoryDbContext();
        _dbContext = dbContext;
        _dbConnection = connection;
    }

    // Dispose method to clean up database resources after each test
    public void Dispose()
    {
        _dbContext.Dispose();
        _dbConnection.Dispose();
    }
    
    [Fact]
    public void AddPoint_IsWorkingCorrectly()
    {
        // Arrange
        Point pointA = new Point(27, 12);
        PointServices pointServices = new(_dbContext);

        // Act
        Point result = pointServices.AddPoint(pointA);

        // Assert
        Assert.Equal(pointA, result);
    }

    [Fact]
    public void GetAllPoints_ReturnsAllPoints()
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
        PointServices pointServices = new(_dbContext);

        // Act
        List<Point> result = pointServices.GetAllPoints();

        // Assert
        result.Should().NotBeEmpty();

        // I'm using equivalent below, because equal comparers if instance is the same, and in this case, i'm only want to check the contents
        result.Should().BeEquivalentTo(dbMock, opt => 
            opt.Excluding(p => p.Id) // Exclude Id property, because it's generated automatically
        );
    }

    [Fact]
    public void GetPointsByProximity_ReturnsCorrectPoints()
    {
        // Arrange
        PointServices pointServices = new(_dbContext);
        int x = 20, y = 10, maxDistance = 10;
        List<Point> awaitedResponse = new() // According the documentation example
        {
            new Point(27, 12, "Cafe"),
            new Point(15, 12, "Jewelry"),
            new Point(12, 8, "Pub"),
            new Point(23, 6, "Supermarket"),
        };

        // Act
        List<Point> result = pointServices.GetPointsByProximity(x, y, maxDistance);

        // Assert
        result.Should().NotBeEmpty();
        result.Should().BeEquivalentTo(awaitedResponse, opt => 
            opt.Excluding(p => p.Id) 
        );
    }
}   