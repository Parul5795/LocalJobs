using LocalJobs.Application.Services;
using LocalJobs.Contracts.Requests;
using LocalJobs.Contracts.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using LocalJobs.Domain.Enums;

namespace LocalJobs.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobsController : ControllerBase
{
    private readonly IJobService _jobService;

    public JobsController(IJobService jobService)
    {
        _jobService = jobService;
    }

    // GET /api/jobs
    [HttpGet]
    public async Task<ActionResult<PagedResponse<JobResponse>>> GetJobs([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        try { return Ok(await _jobService.GetJobsAsync(page, pageSize, cancellationToken)); }
        catch (ArgumentException ex) { return BadRequest(new ProblemDetails { Status = 400, Title = "Invalid pagination", Detail = ex.Message }); }
    }

    // GET /api/jobs/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<JobResponse>> GetJobById(Guid id, CancellationToken cancellationToken)
    {
        var job = await _jobService.GetJobByIdAsync(id, cancellationToken);
        if (job is null)
        {
            return NotFound(new ProblemDetails { Status = 404, Title = "Job not found", Detail = "The job was not found or is inactive." });
        }

        return Ok(job);
    }

    // GET /api/jobs/search?cityId=1&categoryId=2&keyword=driver
    [HttpGet("search")]
    public async Task<ActionResult<PagedResponse<JobResponse>>> SearchJobs(
        [FromQuery] int? cityId,
        [FromQuery] int? categoryId,
        [FromQuery] string? area,
        [FromQuery] string? keyword,
        [FromQuery] string? workType,
        [FromQuery] string? experienceRequirement,
        [FromQuery] string? paymentPeriod,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var jobs = await _jobService.SearchJobsAsync(cityId, categoryId, area, keyword, workType, experienceRequirement, paymentPeriod, page, pageSize, cancellationToken);
            return Ok(jobs);
        }
        catch (ArgumentException ex) { return BadRequest(new ProblemDetails { Status = 400, Title = "Invalid search filters", Detail = ex.Message }); }
    }

    // POST /api/jobs
    [HttpPost]
    [Authorize(Roles = nameof(AccountType.Employer))]
    public async Task<ActionResult<JobResponse>> CreateJob(
        [FromBody] CreateJobRequest request,
        CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userId, out var employerId))
        {
            return Unauthorized();
        }
        try
        {
            var job = await _jobService.CreateJobAsync(request, employerId, cancellationToken);
            if (job is null)
            {
                return BadRequest(new ProblemDetails { Status = 400, Title = "Job could not be created" });
            }

            return CreatedAtAction(nameof(GetJobById), new { id = job.Id }, job);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ProblemDetails { Status = 400, Title = "Invalid job", Detail = ex.Message });
        }
        catch (UnauthorizedAccessException) { return Forbid(); }
    }

    [HttpGet("me")]
    [Authorize(Roles = nameof(AccountType.Employer))]
    public async Task<ActionResult<PagedResponse<JobResponse>>> GetMyJobs([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var employerId)) return Unauthorized();
        try { return Ok(await _jobService.GetMyJobsAsync(employerId, page, pageSize, cancellationToken)); }
        catch (ArgumentException ex) { return BadRequest(new ProblemDetails { Status = 400, Title = "Invalid pagination", Detail = ex.Message }); }
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = nameof(AccountType.Employer))]
    public async Task<ActionResult<JobResponse>> UpdateJob(Guid id, [FromBody] UpdateJobRequest request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var employerId)) return Unauthorized();
        try
        {
            var job = await _jobService.UpdateJobAsync(id, employerId, request, cancellationToken);
            return job is null ? NotFound(new ProblemDetails { Status = 404, Title = "Job not found" }) : Ok(job);
        }
        catch (ArgumentException ex) { return BadRequest(new ProblemDetails { Status = 400, Title = "Invalid job", Detail = ex.Message }); }
    }

    // PUT /api/jobs/{id}/deactivate
    [HttpPut("{id:guid}/deactivate")]
    [Authorize(Roles = nameof(AccountType.Employer))]
    public async Task<IActionResult> DeactivateJob(Guid id, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var employerId)) return Unauthorized();
        var success = await _jobService.DeactivateJobAsync(id, employerId, cancellationToken);
        if (!success)
        {
            return NotFound(new ProblemDetails { Status = 404, Title = "Job not found" });
        }

        return NoContent();
    }
}
