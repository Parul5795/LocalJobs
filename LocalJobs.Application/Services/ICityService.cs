using LocalJobs.Contracts.Responses;

namespace LocalJobs.Application.Services;

public interface ICityService
{
    Task<List<CityResponse>> GetCitiesAsync(CancellationToken cancellationToken = default);
}
