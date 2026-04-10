namespace NorbitWorkingWithDatabasesFromCode.Models;

public class Task
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public int? ProjectId { get; set; }

    public int? StatusId { get; set; }

    public Guid? AssigneeId { get; set; }

    public decimal? EstimatedHours { get; set; }

    public virtual User? Assignee { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual Project? Project { get; set; }

    public virtual TaskStatus? Status { get; set; }

    public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();
}