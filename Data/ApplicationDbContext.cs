using Microsoft.EntityFrameworkCore;
using TaskFlowMvc.Models;

namespace TaskFlowMvc.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Workspace> Workspace => Set<Workspace>();
    public DbSet<Project> Project => Set<Project>();
    public DbSet<TaskState> TaskState => Set<TaskState>();
    public DbSet<Models.Task> Task => base.Set<Models.Task>();
    public DbSet<Tag> Tag => Set<Tag>();
    public DbSet<TaskTag> TaskTag => Set<TaskTag>();

    private void SetAuditFields()
    {
        var entries = ChangeTracker.Entries<BaseEntity>();

        foreach (var entry in entries)
        {

            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }
    }

    public override int SaveChanges()
    {
        SetAuditFields();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SetAuditFields();
        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Workspace>()
             .HasQueryFilter(x => !x.IsDeleted);

        modelBuilder.Entity<Project>()
            .HasQueryFilter(x => !x.IsDeleted);

        modelBuilder.Entity<Models.Task>()
            .HasQueryFilter(x => !x.IsDeleted);

        modelBuilder.Entity<TaskState>()
            .HasQueryFilter(x => !x.IsDeleted);

        modelBuilder.Entity<Tag>()
            .HasQueryFilter(x => !x.IsDeleted);


                modelBuilder.Entity<Project>()
            .HasOne(x => x.Workspace)
            .WithMany(x => x.Projects)
            .HasForeignKey(x => x.WorkspaceId)
            .OnDelete(DeleteBehavior.Restrict);

                modelBuilder.Entity<Models.Task>()
            .HasOne(x => x.Project)
            .WithMany(x => x.Tasks)
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

                modelBuilder.Entity<Models.Task>()
            .HasOne(x => x.TaskState)
            .WithMany(x => x.Tasks)
            .HasForeignKey(x => x.TaskStateId)
            .OnDelete(DeleteBehavior.Restrict);

                modelBuilder.Entity<TaskTag>()
            .HasOne(x => x.Task)
            .WithMany(x => x.TaskTags)
            .HasForeignKey(x => x.TaskId)
            .OnDelete(DeleteBehavior.Restrict);

                modelBuilder.Entity<TaskTag>()
            .HasOne(x => x.Tag)
            .WithMany(x => x.TaskTags)
            .HasForeignKey(x => x.TagId)
            .OnDelete(DeleteBehavior.Restrict);


        modelBuilder.Entity<TaskTag>()
            .HasOne(x => x.Task)
            .WithMany(x => x.TaskTags)
            .HasForeignKey(x => x.TaskId);

        modelBuilder.Entity<TaskTag>()
            .HasOne(x => x.Tag)
            .WithMany(x => x.TaskTags)
            .HasForeignKey(x => x.TagId);
    }
}