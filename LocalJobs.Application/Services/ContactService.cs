using LocalJobs.Application.Interfaces;
using LocalJobs.Contracts.Requests;
using LocalJobs.Contracts.Responses;
using LocalJobs.Domain.Entities;
using LocalJobs.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace LocalJobs.Application.Services;

public class ContactService(IApplicationDbContext context) : IContactService
{
    public async Task<ContactResponse> CreateContactAsync(CreateContactRequest request, Guid employerUserId, CancellationToken cancellationToken = default)
    {
        if (request.WorkerUserId == Guid.Empty || request.WorkerUserId == employerUserId)
            throw new ArgumentException("A valid worker user ID is required and self-contact is not allowed.");
        if (!Enum.TryParse<ContactMethod>(request.Method, true, out var method) || !Enum.IsDefined(method))
            throw new ArgumentException("Method must be Call or WhatsApp.");

        var employer = await context.Users.FirstOrDefaultAsync(u => u.Id == employerUserId, cancellationToken);
        if (employer?.AccountType != AccountType.Employer) throw new UnauthorizedAccessException("Only Employer accounts can initiate contacts.");
        var workerProfile = await context.WorkerProfiles.Include(p => p.User)
            .FirstOrDefaultAsync(p => p.UserId == request.WorkerUserId && p.User.AccountType == AccountType.JobSeeker, cancellationToken);
        if (workerProfile is null) throw new KeyNotFoundException("Worker profile was not found.");

        var contact = new Contact { Id = Guid.NewGuid(), EmployerUserId = employerUserId, WorkerUserId = request.WorkerUserId, Method = method };
        workerProfile.ContactCount++;
        context.Contacts.Add(contact);
        await context.SaveChangesAsync(cancellationToken);
        return new ContactResponse(contact.Id, contact.WorkerUserId, method.ToString(), workerProfile.User.Phone, workerProfile.ContactCount, contact.CreatedAt);
    }
}
