namespace LocalJobs.Domain.Entities;

using LocalJobs.Domain.Common;

public class Job : BaseEntity, IAuditable
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal? Salary { get; set; }

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public int CityId { get; set; }
    public City City { get; set; } = null!;

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ICollection<Application> Applications { get; set; } = new List<Application>();
}
