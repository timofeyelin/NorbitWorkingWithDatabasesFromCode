namespace NorbitWorkingWithDatabasesFromCode.Models;

public class ProjectMember
{
    public int ProjectId { get; set; }

    public Guid UserId { get; set; }

    public string RoleName { get; set; } = null!;

    public virtual Project Project { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}