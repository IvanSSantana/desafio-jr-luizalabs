using GpsPoiApp.Communication.Conversor;
using GpsPoiApp.Communication.Requests;
using GpsPoiApp.Communication.Responses;
using GpsPoiApp.Models;
using GpsPoiApp.Repository;

namespace GpsPoiApp.Services;

public class PointService : IPointService
{
    private readonly IPointRepository _pointRepository;

    public PointService(IPointRepository pointRepository)
    {
        _pointRepository = pointRepository;
    }

    public CreatedPointResponse AddPoint(CreatePointRequest point)
    {
        Point pointRequestToDbModel = RequestConversorToDbModel.ConvertToDbModel(point);
        Point pointFromDb = _pointRepository.AddPoint(pointRequestToDbModel);

        return DbModelConversorToResponse.ConvertToResponse(pointFromDb);
    }

    public GetPointsResponse GetAllPoints()
    {
        List<Point> points = _pointRepository.GetAllPoints();
        GetPointsResponse response = DbModelConversorToResponse.ConvertToResponse(points);

        return response;
    }

    public GetPointsResponse GetPointsByProximity(int x, int y, double maxDistance)
    {
        List<Point> nearbyPoints = _pointRepository.GetPointsByProximity(x, y, maxDistance);
        GetPointsResponse response = DbModelConversorToResponse.ConvertToResponse(nearbyPoints);
        
        return response;
    }
}