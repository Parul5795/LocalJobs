namespace LocalJobs.Domain.Entities;

using LocalJobs.Domain.Common;
using LocalJobs.Domain.Enums;

public class Job : BaseEntity, IAuditable
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal? Salary { get; set; }

    public PaymentPeriod PaymentPeriod { get; set; } = PaymentPeriod.Monthly;

    public string WorkType { get; set; } = string.Empty;

    public string ExperienceRequirement { get; set; } = string.Empty;

    public DateTime? StartDate { get; set; }

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public int CityId { get; set; }
    public City City { get; set; } = null!;

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

}
