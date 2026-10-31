using System.Security.Claims;
using LocalJobs.Application.Services;
using LocalJobs.Contracts.Requests;
using LocalJobs.Contracts.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LocalJobs.Api.Controllers;

[ApiController]
[Route("api/ads")]
public class AdsController(IAdService adService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResponse<AdResponse>>> GetAds([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        try { return Ok(await adService.GetAdsAsync(page, pageSize, cancellationToken)); }
        catch (ArgumentException ex) { return BadRequest(new ProblemDetails { Status = 400, Title = "Invalid pagination", Detail = ex.Message }); }
    }

    [HttpGet("search")]
    public async Task<ActionResult<PagedResponse<AdResponse>>> SearchAds(
        [FromQuery] int? cityId,
        [FromQuery] int? adCategoryId,
        [FromQuery] string? area,
        [FromQuery] string? keyword,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        try { return Ok(await adService.SearchAdsAsync(cityId, adCategoryId, area, keyword, page, pageSize, cancellationToken)); }
        catch (ArgumentException ex) { return BadRequest(new ProblemDetails { Status = 400, Title = "Invalid search filters", Detail = ex.Message }); }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AdResponse>> GetAd(Guid id, CancellationToken cancellationToken)
    {
        var ad = await adService.GetAdByIdAsync(id, cancellationToken);
        return ad is null ? NotFound(new ProblemDetails { Status = 404, Title = "Ad not found" }) : Ok(ad);
    }

    [HttpPost]
    [HttpPost("new")]
    [Authorize]
    public async Task<ActionResult<AdResponse>> CreateAd([FromBody] CreateAdRequest request, CancellationToken cancellationToken)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();
        try
        {
            var ad = await adService.CreateAdAsync(request, userId, cancellationToken);
            return CreatedAtAction(nameof(GetAd), new { id = ad.Id }, ad);
        }
        catch (KeyNotFoundException) { return NotFound(new ProblemDetails { Status = 404, Title = "User not found" }); }
        catch (ArgumentException ex) { return BadRequest(new ProblemDetails { Status = 400, Title = "Invalid ad", Detail = ex.Message }); }
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<PagedResponse<AdResponse>>> GetMyAds([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();
        try { return Ok(await adService.GetMyAdsAsync(userId, page, pageSize, cancellationToken)); }
        catch (ArgumentException ex) { return BadRequest(new ProblemDetails { Status = 400, Title = "Invalid pagination", Detail = ex.Message }); }
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<AdResponse>> UpdateAd(Guid id, [FromBody] UpdateAdRequest request, CancellationToken cancellationToken)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();
        try
        {
            var ad = await adService.UpdateAdAsync(id, request, userId, cancellationToken);
            return ad is null ? NotFound(new ProblemDetails { Status = 404, Title = "Ad not found" }) : Ok(ad);
        }
        catch (ArgumentException ex) { return BadRequest(new ProblemDetails { Status = 400, Title = "Invalid ad", Detail = ex.Message }); }
    }

    [HttpPut("{id:guid}/deactivate")]
    [Authorize]
    public async Task<IActionResult> DeactivateAd(Guid id, CancellationToken cancellationToken)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();
        return await adService.DeactivateAdAsync(id, userId, cancellationToken)
            ? NoContent()
            : NotFound(new ProblemDetails { Status = 404, Title = "Ad not found" });
    }

    private bool TryGetUserId(out Guid userId) => Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out userId);
}
