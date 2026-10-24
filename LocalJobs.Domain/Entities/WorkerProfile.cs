using LocalJobs.Domain.Common;

namespace LocalJobs.Domain.Entities;

public class WorkerProfile : BaseEntity, IAuditable
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public int CityId { get; set; }
    public City City { get; set; } = null!;
    public string? Area { get; set; }
    public string? ProfileImageUrl { get; set; }
    public int ContactCount { get; set; }

    public bool IsFresher { get; set; }
    public decimal? ExperienceYears { get; set; }
    public string ExperienceDetails { get; set; } = string.Empty;
    public string Skills { get; set; } = string.Empty;
    public string Interests { get; set; } = string.Empty;
    public string Availability { get; set; } = string.Empty;
    public string About { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
