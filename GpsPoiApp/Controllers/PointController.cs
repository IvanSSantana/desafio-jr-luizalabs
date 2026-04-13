using GpsPoiApp.Communication.Requests;
using GpsPoiApp.Communication.Responses;
using GpsPoiApp.Models;
using GpsPoiApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace GpsPoiApp.Controllers;

[ApiController]
[Route("api/[controller]/v1")]
public class PointController : ControllerBase
{
    private readonly IPointService _pointServices;

    public PointController(IPointService pointServices)
    {
        _pointServices = pointServices;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Point>))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(MessageErrorResponse))]
    public IActionResult GetAllPoints()
    {   
        GetPointsResponse response = _pointServices.GetAllPoints();

        if (response == null || response.Points.Count == 0) return NoContent();

        return Ok(response);
    }

    [HttpGet("proximity")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Point>))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(MultipleErrorsResponse))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(MultipleErrorsResponse))]
    public IActionResult GetPointsByProximity([FromQuery] int x, [FromQuery] int y, [FromQuery] double maxDistance)
    {
        if (x < 0 || y < 0)
        {
            var errors = new Dictionary<string, string[]>();
            
            if (x < 0)
                errors["X"] = ["X coordinate must be a positive number."];
            if (y < 0)
                errors["Y"] = ["Y coordinate must be a positive number."];

            MultipleErrorsResponse badRequestResponse = new() // RFC 7807 - Problem Details for HTTP APIs
            {
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                Title = "One or more validation errors occurred.",
                Status = StatusCodes.Status400BadRequest,
                Errors = errors,
                TraceId = HttpContext.TraceIdentifier
            };
            return BadRequest(badRequestResponse);  
        } 

        GetPointsResponse response = _pointServices.GetPointsByProximity(x, y, maxDistance);

        if (response == null || response.Points.Count == 0) return NoContent();

        return Ok(response);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Point))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ModelStateDictionary))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(MessageErrorResponse))]
    public IActionResult AddPoint([FromBody] CreatePointRequest point)
    {
        CreatedPointResponse response = _pointServices.AddPoint(point);
        return Created("", response);
    }

    [HttpGet("error")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(MessageErrorResponse))]
    public IActionResult ThrowError()
    {
        throw new Exception("This is a test exception for the global exception handler middleware.");
    }
}
