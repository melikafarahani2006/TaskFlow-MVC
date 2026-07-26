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
    public DbSet<TaskItem> TaskItem => Set<TaskItem>();
    public DbSet<Tag> Tag => Set<Tag>();
    public DbSet<TaskItemTag> TaskItemTag => Set<TaskItemTag>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TaskState>().HasData(
            new TaskState
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Name = "Todo"
            },
            new TaskState
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Name = "In Progress"
            },
            new TaskState
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Name = "Review"
            },
            new TaskState
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                Name = "Done"
            }
        );

        modelBuilder.Entity<TaskItemTag>().HasKey(x => new { x.TaskItemId, x.TagId });
    }
}