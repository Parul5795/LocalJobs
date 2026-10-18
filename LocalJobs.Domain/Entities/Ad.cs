using LocalJobs.Domain.Common;

namespace LocalJobs.Domain.Entities;

public class Ad : BaseEntity, IAuditable
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int AdCategoryId { get; set; }
    public AdCategory AdCategory { get; set; } = null!;
    public int CityId { get; set; }
    public City City { get; set; } = null!;
    public decimal? Price { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
