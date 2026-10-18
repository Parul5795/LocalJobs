using System.Security.Claims;
using LocalJobs.Application.Services;
using LocalJobs.Contracts.Requests;
using LocalJobs.Contracts.Responses;
using LocalJobs.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LocalJobs.Api.Controllers;

[ApiController]
[Route("api/worker-profiles")]
public class WorkerProfilesController(IWorkerProfileService workerProfileService) : ControllerBase
{
    [HttpGet("me")]
    [Authorize(Roles = nameof(AccountType.JobSeeker))]
    public async Task<ActionResult<WorkerProfileResponse>> GetMyProfile(CancellationToken cancellationToken)
    {
        var userId = CurrentUserId();
        if (userId is null) return Unauthorized();
        var profile = await workerProfileService.GetMyProfileAsync(userId.Value, cancellationToken);
        return profile is null ? NotFound() : Ok(profile);
    }

    [HttpPut("me")]
    [Authorize(Roles = nameof(AccountType.JobSeeker))]
    public async Task<ActionResult<WorkerProfileResponse>> SaveMyProfile([FromBody] SaveWorkerProfileRequest request, CancellationToken cancellationToken)
    {
        var userId = CurrentUserId();
        if (userId is null) return Unauthorized();
        try { return Ok(await workerProfileService.SaveMyProfileAsync(userId.Value, request, cancellationToken)); }
        catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
        catch (ArgumentException ex) { return BadRequest(ex.Message); }
        catch (UnauthorizedAccessException) { return Forbid(); }
    }

    [HttpGet("{userId:guid}")]
    [Authorize(Roles = nameof(AccountType.Employer))]
    public async Task<ActionResult<WorkerProfileResponse>> GetPublicProfile(Guid userId, CancellationToken cancellationToken)
    {
        var profile = await workerProfileService.GetPublicProfileAsync(userId, cancellationToken);
        return profile is null ? NotFound() : Ok(profile);
    }

    [HttpGet("search")]
    [Authorize(Roles = nameof(AccountType.Employer))]
    public async Task<ActionResult<List<WorkerSearchResultResponse>>> SearchWorkers(
        [FromQuery] int? categoryId,
        [FromQuery] int? cityId,
        [FromQuery] bool? isFresher,
        [FromQuery] string? keyword,
        CancellationToken cancellationToken)
    {
        var results = await workerProfileService.SearchWorkersAsync(categoryId, cityId, isFresher, keyword, cancellationToken);
        return Ok(results);
    }

    private Guid? CurrentUserId() => Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;
}
