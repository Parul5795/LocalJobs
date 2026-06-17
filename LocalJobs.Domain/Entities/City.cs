namespace LocalJobs.Domain.Entities;

public class City
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string State { get; set; } = string.Empty;

    // Navigation properties
    public ICollection<Job> Jobs { get; set; } = new List<Job>();
}
