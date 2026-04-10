using Microsoft.EntityFrameworkCore;

namespace NorbitWorkingWithDatabasesFromCode.Models;

public partial class EducationDbContext : DbContext
{
    public EducationDbContext()
    {
    }

    public EducationDbContext(DbContextOptions<EducationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<ProjectMember> ProjectMembers { get; set; }

    public virtual DbSet<Tag> Tags { get; set; }

    public virtual DbSet<Task> Tasks { get; set; }

    public virtual DbSet<TaskStatus> TaskStatuses { get; set; }

    public virtual DbSet<User> Users { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Comments__3214EC0768C62E43");

            entity.HasIndex(e => e.AuthorId, "IX_Comments_AuthorId");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Author).WithMany(p => p.Comments)
                .HasForeignKey(d => d.AuthorId)
                .HasConstraintName("FK__Comments__Author__656C112C");

            entity.HasOne(d => d.Task).WithMany(p => p.Comments)
                .HasForeignKey(d => d.TaskId)
                .HasConstraintName("FK__Comments__TaskId__6477ECF3");
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Projects__3214EC074E13A6CF");

            entity.Property(e => e.Budget).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<ProjectMember>(entity =>
        {
            entity.HasKey(e => new { e.ProjectId, e.UserId }).HasName("PK__ProjectM__A762323467F1B39E");

            entity.HasIndex(e => e.UserId, "IX_ProjectMembers_UserId");

            entity.Property(e => e.RoleName).HasMaxLength(50);

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectMembers)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ProjectMe__Proje__52593CB8");

            entity.HasOne(d => d.User).WithMany(p => p.ProjectMembers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ProjectMe__UserI__534D60F1");
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Tags__3214EC075D11A5B7");

            entity.HasIndex(e => e.Name, "UQ__Tags__737584F6E4DD75A7").IsUnique();

            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Task>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Tasks__3214EC0722F4F757");

            entity.HasIndex(e => e.AssigneeId, "IX_Tasks_AssigneeId");

            entity.HasIndex(e => e.ProjectId, "IX_Tasks_ProjectId");

            entity.Property(e => e.EstimatedHours).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.Title).HasMaxLength(200);

            entity.HasOne(d => d.Assignee).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.AssigneeId)
                .HasConstraintName("FK__Tasks__AssigneeI__5AEE82B9");

            entity.HasOne(d => d.Project).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("FK__Tasks__ProjectId__59063A47");

            entity.HasOne(d => d.Status).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK__Tasks__StatusId__59FA5E80");

            entity.HasMany(d => d.Tags).WithMany(p => p.Tasks)
                .UsingEntity<Dictionary<string, object>>(
                    "TaskTag",
                    r => r.HasOne<Tag>().WithMany()
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__TaskTags__TagId__619B8048"),
                    l => l.HasOne<Task>().WithMany()
                        .HasForeignKey("TaskId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__TaskTags__TaskId__60A75C0F"),
                    j =>
                    {
                        j.HasKey("TaskId", "TagId").HasName("PK__TaskTags__AA3E862B9C77B71A");
                        j.ToTable("TaskTags");
                    });
        });

        modelBuilder.Entity<TaskStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TaskStat__3214EC0743DD1539");

            entity.HasIndex(e => e.Name, "UQ__TaskStat__737584F6E5DC95D3").IsUnique();

            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC07A5CC75A2");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D105341706792E").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}