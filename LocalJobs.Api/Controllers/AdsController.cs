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
    public async Task<ActionResult<List<AdResponse>>> GetAds(CancellationToken cancellationToken) =>
        Ok(await adService.GetAdsAsync(cancellationToken));

    [HttpGet("search")]
    public async Task<ActionResult<List<AdResponse>>> SearchAds(
        [FromQuery] int? cityId,
        [FromQuery] int? adCategoryId,
        [FromQuery] string? keyword,
        CancellationToken cancellationToken) =>
        Ok(await adService.SearchAdsAsync(cityId, adCategoryId, keyword, cancellationToken));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AdResponse>> GetAd(Guid id, CancellationToken cancellationToken)
    {
        var ad = await adService.GetAdByIdAsync(id, cancellationToken);
        return ad is null ? NotFound() : Ok(ad);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<AdResponse>> CreateAd([FromBody] CreateAdRequest request, CancellationToken cancellationToken)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();
        try
        {
            var ad = await adService.CreateAdAsync(request, userId, cancellationToken);
            return CreatedAtAction(nameof(GetAd), new { id = ad.Id }, ad);
        }
        catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
        catch (ArgumentException ex) { return BadRequest(ex.Message); }
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<List<AdResponse>>> GetMyAds(CancellationToken cancellationToken)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();
        return Ok(await adService.GetMyAdsAsync(userId, cancellationToken));
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<AdResponse>> UpdateAd(Guid id, [FromBody] UpdateAdRequest request, CancellationToken cancellationToken)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();
        try
        {
            var ad = await adService.UpdateAdAsync(id, request, userId, cancellationToken);
            return ad is null ? NotFound() : Ok(ad);
        }
        catch (ArgumentException ex) { return BadRequest(ex.Message); }
    }

    [HttpPut("{id:guid}/deactivate")]
    [Authorize]
    public async Task<IActionResult> DeactivateAd(Guid id, CancellationToken cancellationToken)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();
        return await adService.DeactivateAdAsync(id, userId, cancellationToken) ? NoContent() : NotFound();
    }

    private bool TryGetUserId(out Guid userId) => Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out userId);
}
