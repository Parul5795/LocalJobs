using LocalJobs.Application.Services;
using LocalJobs.Contracts.Responses;
using Microsoft.AspNetCore.Mvc;

namespace LocalJobs.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    // GET /api/categories
    [HttpGet]
    public async Task<ActionResult<List<CategoryResponse>>> GetCategories(CancellationToken cancellationToken)
    {
        var categories = await _categoryService.GetCategoriesAsync(cancellationToken);
        return Ok(categories);
    }
}
