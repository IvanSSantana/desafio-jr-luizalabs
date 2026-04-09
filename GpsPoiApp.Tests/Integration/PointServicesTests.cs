using FluentAssertions;
using GpsPoiApp.Helper;
using GpsPoiApp.Infrastructure;
using GpsPoiApp.Models;
using GpsPoiApp.Services;
using GpsPoiApp.Repository;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace GpsPoiApp.Tests;

public class PointRepositoryTests : IDisposable
{
    private readonly AppDbContext _dbContext;
    private readonly SqliteConnection _dbConnection;

    // Setup method to create an in-memory database context for every testing
    public PointRepositoryTests()
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
        PointRepository PointRepository = new(_dbContext);

        // Act
        Point result = PointRepository.AddPoint(pointA);
        List<Point> allPoints = PointRepository.GetAllPoints();

        // Assert
        result.Should().NotBeNull();

        result.Should().BeEquivalentTo(pointA, opt => 
            opt.Excluding(p => p.Id) 
        );
        
        allPoints.Should().ContainSingle(p => p.X == pointA.X && p.Y == pointA.Y);
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
        PointRepository PointRepository = new(_dbContext);

        // Act
        List<Point> result = PointRepository.GetAllPoints();

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
        PointRepository PointRepository = new(_dbContext);
        int x = 20, y = 10, maxDistance = 10;
        List<Point> awaitedResponse = new() // According the documentation example
        {
            new Point(27, 12, "Cafe"),
            new Point(15, 12, "Jewelry"),
            new Point(12, 8, "Pub"),
            new Point(23, 6, "Supermarket"),
        };

        // Act
        List<Point> result = PointRepository.GetPointsByProximity(x, y, maxDistance);

        // Assert
        result.Should().NotBeEmpty();
        result.Should().BeEquivalentTo(awaitedResponse, opt => 
            opt.Excluding(p => p.Id) 
        );
    }
}   