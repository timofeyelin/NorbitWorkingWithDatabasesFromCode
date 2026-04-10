namespace NorbitWorkingWithDatabasesFromCode.Models;

public class Comment
{
    public int Id { get; set; }

    public int? TaskId { get; set; }

    public Guid? AuthorId { get; set; }

    public string? Text { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual User? Author { get; set; }

    public virtual Task? Task { get; set; }
}