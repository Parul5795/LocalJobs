using LocalJobs.Application.Services;
using LocalJobs.Contracts.Responses;
using Microsoft.AspNetCore.Mvc;

namespace LocalJobs.Api.Controllers;

[ApiController]
[Route("api/ad-categories")]
public class AdCategoriesController(IAdService adService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<AdCategoryResponse>>> GetCategories(CancellationToken cancellationToken) =>
        Ok(await adService.GetCategoriesAsync(cancellationToken));
}
