using GpsPoiApp.Communication.Requests;
using GpsPoiApp.Communication.Responses;
using GpsPoiApp.Models;

namespace GpsPoiApp.Services;

public interface IPointService
{
    CreatedPointResponse AddPoint(CreatePointRequest point);
    GetPointsResponse GetAllPoints();
    GetPointsResponse GetPointsByProximity(int x, int y, double maxDistance);
}
