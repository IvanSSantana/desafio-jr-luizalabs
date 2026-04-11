using GpsPoiApp.Communication.Responses;
using GpsPoiApp.Communication.Requests;
using GpsPoiApp.Models;

namespace GpsPoiApp.Communication.Conversor;

public static class ResponseConversorToDbModel
{
    public static Point ConvertToDbModel(CreatePointRequest request)
    {
        return new Point(request.X, request.Y, request.Name);
    }
}

public static class DbModelConversorToResponse
{
    public static CreatedPointResponse ConvertToResponse(Point point)
    {
        return new CreatedPointResponse { X = point.X, Y = point.Y, Name = point.Name! };
    }

    public static GetPointsResponse ConvertToResponse(List<Point> points)
    {
        return new GetPointsResponse { 
            Points = points.Select(p => new Point(p.X, p.Y, p.Name) { Id = p.Id }).ToList() 
        };
    }
}