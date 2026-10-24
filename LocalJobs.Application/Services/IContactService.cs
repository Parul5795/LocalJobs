using LocalJobs.Contracts.Requests;
using LocalJobs.Contracts.Responses;

namespace LocalJobs.Application.Services;

public interface IContactService
{
    Task<ContactResponse> CreateContactAsync(CreateContactRequest request, Guid employerUserId, CancellationToken cancellationToken = default);
}
