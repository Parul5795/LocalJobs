namespace LocalJobs.Domain.Entities;

using LocalJobs.Domain.Common;
using LocalJobs.Domain.Enums;

public class User : BaseEntity, IAuditable
{
    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public AccountType AccountType { get; set; } = AccountType.JobSeeker;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ICollection<Job> PostedJobs { get; set; } = new List<Job>();

    public WorkerProfile? WorkerProfile { get; set; }
    
}
