namespace NorbitWorkingWithDatabasesFromCode.Models;

public class Project
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public decimal? Budget { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<ProjectMember> ProjectMembers { get; set; } = new List<ProjectMember>();

    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
}