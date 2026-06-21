using LocalJobs.Application.Services;
using LocalJobs.Contracts.Responses;
using Microsoft.AspNetCore.Mvc;

namespace LocalJobs.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CitiesController : ControllerBase
{
    private readonly ICityService _cityService;

    public CitiesController(ICityService cityService)
    {
        _cityService = cityService;
    }

    // GET /api/cities
    [HttpGet]
    public async Task<ActionResult<List<CityResponse>>> GetCities(CancellationToken cancellationToken)
    {
        var cities = await _cityService.GetCitiesAsync(cancellationToken);
        return Ok(cities);
    }
}
