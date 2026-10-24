using LocalJobs.Domain.Common;
using LocalJobs.Domain.Enums;

namespace LocalJobs.Domain.Entities;

public class Contact : BaseEntity
{
    public Guid WorkerUserId { get; set; }
    public Guid EmployerUserId { get; set; }
    public ContactMethod Method { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
