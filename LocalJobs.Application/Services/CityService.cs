using LocalJobs.Application.Interfaces;
using LocalJobs.Contracts.Responses;
using Microsoft.EntityFrameworkCore;

namespace LocalJobs.Application.Services;

public class CityService : ICityService
{
    private readonly IApplicationDbContext _context;

    public CityService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<CityResponse>> GetCitiesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Cities
            .OrderBy(c => c.Name)
            .Select(c => new CityResponse(c.Id, c.Name, c.State))
            .ToListAsync(cancellationToken);
    }
}
