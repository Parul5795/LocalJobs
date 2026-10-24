using System.Security.Claims;
using LocalJobs.Application.Services;
using LocalJobs.Contracts.Requests;
using LocalJobs.Contracts.Responses;
using LocalJobs.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LocalJobs.Api.Controllers;

[ApiController]
[Route("api/contacts")]
[Authorize(Roles = nameof(AccountType.Employer))]
public class ContactsController(IContactService contactService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<ContactResponse>> Create([FromBody] CreateContactRequest request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var employerId)) return Unauthorized();
        try { return Ok(await contactService.CreateContactAsync(request, employerId, cancellationToken)); }
        catch (KeyNotFoundException) { return NotFound(new ProblemDetails { Status = 404, Title = "Worker profile not found" }); }
        catch (UnauthorizedAccessException) { return Forbid(); }
        catch (ArgumentException ex) { return BadRequest(new ProblemDetails { Status = 400, Title = "Invalid contact request", Detail = ex.Message }); }
    }
}
