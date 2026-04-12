using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using GpsPoiApp.Helper;
using GpsPoiApp.Communication.Requests;
using GpsPoiApp.Communication.Responses;
using GpsPoiApp.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using GpsPoiApp.Models;

namespace GpsPoiApp.Tests.E2E.Controllers;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connection;
 
    public CustomWebApplicationFactory()
    {
        (AppDbContext context, SqliteConnection connection) = TestDbFactory.CreateInMemoryDbContext();
        _connection = connection;
        context.Dispose();
    }
 
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
 
        builder.ConfigureServices(services =>
        {
            ServiceDescriptor? descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
 
            // Replace real database for in-memory SQLite for testing
            if (descriptor is not null)
                services.Remove(descriptor);
 
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlite(_connection);
            });
        });
    }
 
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        _connection.Dispose();
    }
}

[Collection("NonParallel")]
public class PointControllerE2ETests : IClassFixture<CustomWebApplicationFactory> // For shared database beetwen tests, simulating the real application
{
    private const string BaseRoute = "/api/point/v1";
    private const string ProximityRoute = $"{BaseRoute}/proximity";
    private const string ErrorRoute = $"{BaseRoute}/error";
    private HttpClient client;

    public PointControllerE2ETests(CustomWebApplicationFactory factory)
    {
        client = factory.CreateClient();;
    }

    private HttpClient CreateIsolatedClient(Action<AppDbContext>? seed = null)
    {
        var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Testing");

                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

                    if (descriptor != null)
                        services.Remove(descriptor);

                    var connection = new SqliteConnection("DataSource=:memory:");
                    connection.Open();

                    services.AddDbContext<AppDbContext>(options =>
                    {
                        options.UseSqlite(connection);
                    });

                    var serviceProvider = services.BuildServiceProvider();
                    using var serviceProviderScope = serviceProvider.CreateScope();
 
                    var db = serviceProviderScope.ServiceProvider.GetRequiredService<AppDbContext>();

                    db.Database.EnsureDeleted();
                    db.Database.EnsureCreated();

                    seed?.Invoke(db);
                });
            });

        return factory.CreateClient();
    }

    #region GetAllPoints

    [Fact]
    public async Task GetAllPoints_ResponseBody_ContainsCorrectPointData()
    {
        // Arrange
        // I isolated the database here, because the 'AddPoint' tests were interfering on the database
        var isolatedClient = CreateIsolatedClient(db =>
        {
            db.Points.AddRange(
                new Point(27, 12, "Cafe"),
                new Point(31, 18, "Gas Station"),
                new Point(15, 12, "Jewelry"),
                new Point(19, 21, "Floriculture"),
                new Point(12, 8, "Pub"),
                new Point(23, 6, "Supermarket"),
                new Point(28, 2, "Steakhouse")
            );
            db.SaveChanges();
        });

        List<CreatedPointResponse> expectedResponse = new()
        {
            new CreatedPointResponse { X = 27, Y = 12, Name = "Cafe" },
            new CreatedPointResponse { X = 31, Y = 18, Name = "Gas Station" },
            new CreatedPointResponse { X = 15, Y = 12, Name = "Jewelry" },
            new CreatedPointResponse { X = 19, Y = 21, Name = "Floriculture" },
            new CreatedPointResponse { X = 12, Y = 8, Name = "Pub" },
            new CreatedPointResponse { X = 23, Y = 6, Name = "Supermarket" },
            new CreatedPointResponse { X = 28, Y = 2, Name = "Steakhouse" },
        };

        // Act
        HttpResponseMessage response = await client.GetAsync(BaseRoute);
        GetPointsResponse? body = await response.Content.ReadFromJsonAsync<GetPointsResponse>();

        // Assert 
        body.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        body.Points.Should().HaveCount(7);
        body!.Points.Should().BeEquivalentTo(expectedResponse, opt => opt.Excluding(p => p.Id));
    }

    [Fact]
    public async Task GetAllPoints_WhenDatabaseIsEmpty_Returns204NoContent()
    {
        // Arrange
        var isolatedClient = CreateIsolatedClient();

        // Act
        HttpResponseMessage response = await isolatedClient.GetAsync(BaseRoute);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task GetAllPoints_AfterInsertingPoint_ReturnedListIncludesNewPoint()
    {
        var request = new CreatePointRequest { Name = "Library", X = 10, Y = 10 };
        await client.PostAsJsonAsync(BaseRoute, request);

        // Act
        HttpResponseMessage response = await client.GetAsync(BaseRoute);
        GetPointsResponse? body = await response.Content.ReadFromJsonAsync<GetPointsResponse>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        body!.Points.Should().ContainSingle(p => p.Name == "Library" && p.X == 10 && p.Y == 10);
    }

    #endregion

    #region GetPointsByProximity

    [Fact]
    public async Task GetPointsByProximity_WithKnownCoordinates_ReturnsCorrectPoints()
    {
        // Arrange
        var isolatedClient = CreateIsolatedClient(db =>
        {
            db.Points.AddRange(
                new Point(27, 12, "Cafe"),
                new Point(31, 18, "Gas Station"),
                new Point(15, 12, "Jewelry"),
                new Point(19, 21, "Floriculture"),
                new Point(12, 8, "Pub"),
                new Point(23, 6, "Supermarket"),
                new Point(28, 2, "Steakhouse")
            );
            db.SaveChanges();
        });

        List<CreatedPointResponse> awaitedResponse = new() // According the documentation example
        {
            new CreatedPointResponse { X = 27, Y = 12, Name = "Cafe" },
            new CreatedPointResponse { X = 15, Y = 12, Name = "Jewelry" },
            new CreatedPointResponse { X = 12, Y = 8, Name = "Pub" },
            new CreatedPointResponse { X = 23, Y = 6, Name = "Supermarket" }
        };

        // Act
        HttpResponseMessage response = await isolatedClient.GetAsync(
            $"{ProximityRoute}?x=20&y=10&maxDistance=10");
        GetPointsResponse? body = await response.Content.ReadFromJsonAsync<GetPointsResponse>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        body!.Points.Should().HaveCount(4);
        body.Points.Should().BeEquivalentTo(awaitedResponse, opt => opt.Excluding(p => p.Id));
    }

    [Fact]
    public async Task GetPointsByProximity_Returns204WhenNothingIsNear()
    {
        // Act — origin (0,0) with d=1 catches nothing in the seed dataset
        HttpResponseMessage response = await client.GetAsync(
            $"{ProximityRoute}?x=0&y=0&maxDistance=1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Theory] // Applying BDD
    [InlineData("?x=abc&y=10&maxDistance=5")]  // x is not a number
    [InlineData("?x=10&y=abc&maxDistance=5")]  // y is not a number
    [InlineData("?x=-1&y=10&maxDistance=5")]   // x is a negative number
    [InlineData("?x=10&y=-1&maxDistance=5")]   // y is a negative number
    [InlineData("?x=10&y=10&maxDistance=abc")] // maxDistance is not a number
    public async Task GetPointsByProximity_WithInvalidQueryParams_Returns400BadRequest(string queryString)
    {
        HttpResponseMessage response = await client.GetAsync($"{ProximityRoute}{queryString}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region AddPoint
    
    [Fact]
    public async Task AddPoint_WithValidPayload_Returns201Created()
    {
        var request = new CreatePointRequest { Name = "Hospital", X = 10, Y = 5 };

        // Act
        HttpResponseMessage response = await client.PostAsJsonAsync(BaseRoute, request);
        CreatedPointResponse? body = await response.Content.ReadFromJsonAsync<CreatedPointResponse>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        body.Should().NotBeNull();
        body.Id.Should().NotBe(Guid.Empty);
        request.Should().BeEquivalentTo(body, opt => opt.Excluding(p => p.Id));
    }

    [Fact]
    public async Task AddPoint_IsPersitingOnDatabase()
    {
        var request = new CreatePointRequest { Name = "Museum", X = 44, Y = 22 };

        // Act
        HttpResponseMessage postJson = await client.PostAsJsonAsync(BaseRoute, request);
        CreatedPointResponse? postResponse = await postJson.Content.ReadFromJsonAsync<CreatedPointResponse>();

        HttpResponseMessage getJson = await client.GetAsync(BaseRoute);
        GetPointsResponse?  getResponse = await getJson.Content.ReadFromJsonAsync<GetPointsResponse>();

        // Assert
        getResponse!.Points.Should().ContainSingle(p =>
            p.Id == postResponse!.Id &&
            p.Name == "Museum" &&
            p.X == 44 &&
            p.Y == 22
        );
    }

    [Fact]
    public async Task AddPoint_WithEmptyBody_Returns400BadRequest()
    {
        // Act
        HttpResponseMessage response = await client.PostAsync(
            BaseRoute,
            new StringContent(string.Empty, System.Text.Encoding.UTF8, "application/json"));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AddPoint_WithMissingNameField_Returns400BadRequest()
    {
        // Act
        HttpResponseMessage response = await client.PostAsJsonAsync(BaseRoute, new { X = 10, Y = 5 });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData(-1,  5)]  
    [InlineData( 5, -1)]  
    [InlineData(-1, -1)]  
    public async Task AddPoint_WithNegativeCoordinates_Returns400BadRequest(int x, int y)
    {
        // Act
        HttpResponseMessage response = await client.PostAsJsonAsync(
            BaseRoute, new { Name = "Inválido", X = x, Y = y });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region ErrorHandlingMiddlewares

    [Fact]
    public async Task ThrowError_Always_Returns500InternalServerError()
    {
        // Act
        HttpResponseMessage response = await client.GetAsync(ErrorRoute);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task ThrowError_ResponseBody_ContainsErrorInformation()
    {
        // Arrange
        MessageErrorResponse expectedResponse = new() // According to RFC 7807 - Problem Details for HTTP APIs
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            Title = "Internal Server Error",
            Status = StatusCodes.Status500InternalServerError,
            Message = "An unexpected error occurred. Please try again later."
        };

        // Act
        HttpResponseMessage response = await client.GetAsync(ErrorRoute);
        MessageErrorResponse? body = await response.Content.ReadFromJsonAsync<MessageErrorResponse>();

        // Assert
        body.Should().NotBeNull();
        body.Should().BeEquivalentTo(expectedResponse, opt => opt.Excluding(er => er.TraceId));
    }

    [Fact]
    public async Task UnknownRoute_Returns404NotFound()
    {
        // Act
        HttpResponseMessage response = await client.GetAsync($"{BaseRoute}/does-not-exist");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    #endregion
}