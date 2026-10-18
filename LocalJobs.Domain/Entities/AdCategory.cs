namespace LocalJobs.Domain.Entities;

public class AdCategory
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<Ad> Ads { get; set; } = new List<Ad>();
}
