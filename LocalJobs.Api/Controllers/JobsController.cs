using LocalJobs.Application.Services;
using LocalJobs.Contracts.Requests;
using LocalJobs.Contracts.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LocalJobs.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobsController : ControllerBase
{
    // Temporary MVP UserId — will be replaced by JWT claim once auth is added
    private static readonly Guid MvpUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");

    private readonly IJobService _jobService;

    public JobsController(IJobService jobService)
    {
        _jobService = jobService;
    }

    // GET /api/jobs
    [HttpGet]
    public async Task<ActionResult<List<JobResponse>>> GetJobs(CancellationToken cancellationToken)
    {
        var jobs = await _jobService.GetJobsAsync(cancellationToken);
        return Ok(jobs);
    }

    // GET /api/jobs/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<JobResponse>> GetJobById(Guid id, CancellationToken cancellationToken)
    {
        var job = await _jobService.GetJobByIdAsync(id, cancellationToken);
        if (job is null)
        {
            return NotFound($"Job with ID '{id}' was not found or is inactive.");
        }

        return Ok(job);
    }

    // GET /api/jobs/search?cityId=1&categoryId=2
    [HttpGet("search")]
    public async Task<ActionResult<List<JobResponse>>> SearchJobs(
        [FromQuery] int? cityId,
        [FromQuery] int? categoryId,
        CancellationToken cancellationToken)
    {
        var jobs = await _jobService.SearchJobsAsync(cityId, categoryId, cancellationToken);
        return Ok(jobs);
    }

    // POST /api/jobs
    [HttpPost]
    public async Task<ActionResult<JobResponse>> CreateJob(
        [FromBody] CreateJobRequest request,
        CancellationToken cancellationToken)
    {
        // MVP: userId is hardcoded. Replace with JWT claim (HttpContext.User) later.
        var userId = MvpUserId;

        try
        {
            var job = await _jobService.CreateJobAsync(request, userId, cancellationToken);
            if (job is null)
            {
                return BadRequest("Job could not be created.");
            }

            return CreatedAtAction(nameof(GetJobById), new { id = job.Id }, job);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // PUT /api/jobs/{id}/deactivate
    [HttpPut("{id:guid}/deactivate")]
    public async Task<IActionResult> DeactivateJob(Guid id, CancellationToken cancellationToken)
    {
        var success = await _jobService.DeactivateJobAsync(id, cancellationToken);
        if (!success)
        {
            return NotFound($"Job with ID '{id}' was not found.");
        }

        return NoContent();
    }
}
