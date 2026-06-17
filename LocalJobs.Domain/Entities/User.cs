namespace LocalJobs.Domain.Entities;

using LocalJobs.Domain.Common;

public class User : BaseEntity, IAuditable
{
    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ICollection<Job> PostedJobs { get; set; } = new List<Job>();
    
    public ICollection<Application> Applications { get; set; } = new List<Application>();
}