using FluentAssertions;
using GpsPoiApp.Communication.Requests;
using GpsPoiApp.Communication.Responses;
using GpsPoiApp.Models;
using GpsPoiApp.Repository;
using GpsPoiApp.Services;
using Moq;

namespace GpsPoiApp.Tests;

public class PointServiceTests
{
    private readonly Mock<IPointRepository> _pointRepositoryMock;
    private readonly PointService _pointService;

    public PointServiceTests()
    {
        _pointRepositoryMock = new Mock<IPointRepository>();
        _pointService = new PointService(_pointRepositoryMock.Object);
    }

    [Fact]
    public void AddPoint_ShouldConvertRequestAndReturnResponse()
    {
        // Arrange
        CreatePointRequest requestPoint = new() { X = 10, Y = 20, Name = "Test Point" };
        Point dbPoint = new(10, 20, "Test Point") { Id = new Guid("00000000-0000-0000-0000-000000000001") };

        _pointRepositoryMock
        .Setup(repo => repo.AddPoint(It.IsAny<Point>()))
        .Returns(dbPoint);

        // Act
        CreatedPointResponse result = _pointService.AddPoint(requestPoint);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(dbPoint);
        _pointRepositoryMock.Verify(r => r.AddPoint(It.IsAny<Point>()), Times.Once);
    }

    [Fact]
    public void GetAllPoints_ShouldConvertDbModelsAndReturnResponse()
    {
        // Arrange
        List<Point> mockedDb = new()
        {
            new Point(27, 12, "Cafe") { Id = new Guid("00000000-0000-0000-0000-000000000001") },
            new Point(31, 18, "Gas Station") { Id = new Guid("00000000-0000-0000-0000-000000000002") },
            new Point(15, 12, "Jewelry") { Id = new Guid("00000000-0000-0000-0000-000000000003") }
        };
        _pointRepositoryMock.Setup(r => r.GetAllPoints()).Returns(mockedDb);

        // Act
        GetPointsResponse result = _pointService.GetAllPoints();

        // Assert
        result.Should().NotBeNull();
        result.Points.Should().NotBeEmpty();
        result.Points.Should().HaveCount(3);
        result.Points.Should().BeEquivalentTo(mockedDb);
        
        _pointRepositoryMock.Verify(r => r.GetAllPoints(), Times.Once);
    }

    [Fact]
    public void GetAllPoints_ShouldReturnEmptyResponseWhenNoPointsExist()
    {
        // Arrange
        _pointRepositoryMock.Setup(r => r.GetAllPoints()).Returns(new List<Point>());

        // Act
        GetPointsResponse result = _pointService.GetAllPoints();

        // Assert
        result.Should().NotBeNull();
        result.Points.Should().BeEmpty();
        _pointRepositoryMock.Verify(r => r.GetAllPoints(), Times.Once);
    }

    [Fact]
    public void GetPointsByProximity_ShouldReturnConvertedResponse()
    {
        // Arrange
        List<Point> nearbyPoints = new()
        {
            new Point(27, 12, "Cafe") { Id = new Guid("00000000-0000-0000-0000-000000000001") },
            new Point(15, 12, "Jewelry") { Id = new Guid("00000000-0000-0000-0000-000000000003") },
            new Point(23, 6, "Supermarket") { Id = new Guid("00000000-0000-0000-0000-000000000006") }
        };

        int x = 20, y = 10;
        double maxDistance = 10;
        _pointRepositoryMock.Setup(r => r.GetPointsByProximity(x, y, maxDistance)).Returns(nearbyPoints);

        // Act
        GetPointsResponse result = _pointService.GetPointsByProximity(x, y, maxDistance);

        // Assert
        result.Should().NotBeNull();
        result.Points.Should().NotBeEmpty();
        result.Points.Should().HaveCount(3);
        result.Points.Should().BeEquivalentTo(nearbyPoints);
        _pointRepositoryMock.Verify(r => r.GetPointsByProximity(x, y, maxDistance), Times.Once);
    }

    [Fact]
    public void GetPointsByProximity_ShouldReturnEmptyResponseWhenNoNearbyPointsExist()
    {
        // Arrange
        int x = 100, y = 100;
        double maxDistance = 5;
        _pointRepositoryMock.Setup(r => r.GetPointsByProximity(x, y, maxDistance)).Returns(new List<Point>());

        // Act
        GetPointsResponse result = _pointService.GetPointsByProximity(x, y, maxDistance);

        // Assert
        result.Should().NotBeNull();
        result.Points.Should().BeEmpty();
        _pointRepositoryMock.Verify(r => r.GetPointsByProximity(x, y, maxDistance), Times.Once);
    }
}