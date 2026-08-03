using Microsoft.EntityFrameworkCore;
using Quartz;
using TaskFlowMvc.Data;

namespace TaskFlowMvc.Jobs;

public class OverdueTaskCheckJob : IJob
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OverdueTaskCheckJob> _logger;

    public OverdueTaskCheckJob(IServiceScopeFactory scopeFactory, ILogger<OverdueTaskCheckJob> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var thresholdDate = DateTime.UtcNow.AddMinutes(-1);

        var staleTasks = await dbContext.Task
            .Include(t => t.TaskState)
            .Include(t => t.Project)
            .Where(t => t.TaskState.Name == "In Progress")
            .Where(t => (t.UpdatedAt ?? t.CreatedAt) < thresholdDate)
            .ToListAsync();

        if (staleTasks.Count == 0)
        {
            _logger.LogInformation("Overdue task check: no stale tasks found.");
            return;
        }

        foreach (var task in staleTasks)
        {
            _logger.LogWarning(
                "Task '{Title}' in project '{Project}' has been In Progress since {LastUpdate} without update.",
                task.Title, task.Project.Name, task.UpdatedAt ?? task.CreatedAt);
        }
    }
}