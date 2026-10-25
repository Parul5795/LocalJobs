using System.Security.Claims;
using LocalJobs.Application.Services;
using LocalJobs.Contracts.Requests;
using LocalJobs.Contracts.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LocalJobs.Api.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UsersController(IUserService userService) : ControllerBase
{
    [HttpGet("me")]
    public async Task<ActionResult<UserResponse>> GetMe(CancellationToken cancellationToken)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();
        var user = await userService.GetByIdAsync(userId, cancellationToken);
        return user is null ? NotFound(new ProblemDetails { Status = 404, Title = "User not found" }) : Ok(user);
    }

    [HttpPut("me")]
    public async Task<ActionResult<UserResponse>> UpdateMe([FromBody] UpdateUserRequest request, CancellationToken cancellationToken)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();
        try { return Ok(await userService.UpdateAsync(userId, request, cancellationToken)); }
        catch (ArgumentException ex) { return BadRequest(new ProblemDetails { Status = 400, Title = "Invalid user information", Detail = ex.Message }); }
        catch (UserEmailConflictException ex) { return Conflict(new ProblemDetails { Status = 409, Title = "Email already registered", Detail = ex.Message }); }
        catch (KeyNotFoundException) { return NotFound(new ProblemDetails { Status = 404, Title = "User not found" }); }
    }

    private bool TryGetUserId(out Guid userId) =>
        Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub"), out userId);
}
