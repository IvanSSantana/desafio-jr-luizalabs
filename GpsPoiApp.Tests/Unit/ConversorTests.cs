using FluentAssertions;
using GpsPoiApp.Communication.Conversor;
using GpsPoiApp.Communication.Requests;
using GpsPoiApp.Communication.Responses;
using GpsPoiApp.Models;

namespace GpsPoiApp.Tests;

public class ConversorTests
{
    [Fact]
    public void RequestConversorToDbModel_ShouldConvertRequestToDbModel()
    {
        // Arrange 
        CreatePointRequest requestModel = new() { X = 10, Y = 20, Name = "Test Point" };
        Point expectedDbModel = new(10, 20, "Test Point");

        // Act
        Point dbModel = RequestConversorToDbModel.ConvertToDbModel(requestModel);

        // Assert 
        dbModel.Should().NotBeNull();
        dbModel.Should().BeEquivalentTo(expectedDbModel, options => options.Excluding(p => p.Id));
        dbModel.Id.Should().NotBeEmpty(); // Assuming Id is not set during conversion and defaults to 0
    }

    [Fact]
    public void DbModelConversorToResponse_ShouldConvertDbModelToSingleResponse()
    {
        // Arrange
        Point dbModel = new(10, 20, "Test Point");
        CreatedPointResponse expectedResponse = new() { X = 10, Y = 20, Name = "Test Point", Id = dbModel.Id };

        // Act
        CreatedPointResponse response = DbModelConversorToResponse.ConvertToResponse(dbModel);

        // Assert
        response.Should().NotBeNull();
        response.Should().BeEquivalentTo(expectedResponse);
    }

    [Fact]
    public void DbModelConversorToResponse_ShouldConvertListDbModelToListResponse()
    {
        // Arrange
        List<Point> dbModels = new() { new(10, 20, "Test Point"), new(30, 40, "Another Point") };
        List<CreatedPointResponse> expectedPoints = 
        [
            new() { X = 10, Y = 20, Name = "Test Point", Id = dbModels[0].Id },
            new() { X = 30, Y = 40, Name = "Another Point", Id = dbModels[1].Id }
        ];
        
        GetPointsResponse expectedResponse = new() { Points = expectedPoints };

        // Act
        GetPointsResponse response = DbModelConversorToResponse.ConvertToResponse(dbModels);

        // Assert
        response.Should().NotBeNull();
        response.Should().BeEquivalentTo(expectedResponse);
    }
}
