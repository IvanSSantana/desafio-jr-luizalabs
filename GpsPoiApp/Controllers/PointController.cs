using GpsPoiApp.Models;
using GpsPoiApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace GpsPoiApp.Controllers;

[ApiController]
[Route("[controller]")]
public class PointController : ControllerBase
{
    private readonly PointServices _pointServices;

    public PointController(PointServices pointServices)
    {
        _pointServices = pointServices;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Point>))]
    public IActionResult GetAllPoints()
    {   
        List<Point> response = _pointServices.GetAllPoints();
        return Ok(response);
    }

    [HttpGet("proximity")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Point>))]
    public IActionResult GetPointsByProximity([FromQuery] int x, [FromQuery] int y, [FromQuery] double maxDistance)
    {
        List<Point> response = _pointServices.GetPointsByProximity(x, y, maxDistance);
        return Ok(response);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Point))]
    public IActionResult AddPoint([FromBody] Point point)
    {
        Point response = _pointServices.AddPoint(point);
        return Created("", response);
    }
}
