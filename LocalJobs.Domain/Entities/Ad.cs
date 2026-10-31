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
    public string? Area { get; set; }
    public string ContactPersonName { get; set; } = string.Empty;
    public string ContactMobileNumber { get; set; } = string.Empty;
    public bool AllowPhoneCall { get; set; }
    public bool AllowWhatsApp { get; set; }
    public string[] ImageUrls { get; set; } = [];
    public decimal? Price { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
