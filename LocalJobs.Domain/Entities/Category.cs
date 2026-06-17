namespace LocalJobs.Domain.Entities;

public class Category
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    // Navigation properties
    public ICollection<Job> Jobs { get; set; } = new List<Job>();
}
