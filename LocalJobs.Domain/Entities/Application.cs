namespace LocalJobs.Domain.Entities;

using LocalJobs.Domain.Common;
using LocalJobs.Domain.Enums;

public class Application : BaseEntity, IAuditable
{
    public Guid JobId { get; set; }
    public Job Job { get; set; } = null!;

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public string CoverLetter { get; set; } = string.Empty;

    public ApplicationStatus Status { get; set; } = ApplicationStatus.Applied;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}