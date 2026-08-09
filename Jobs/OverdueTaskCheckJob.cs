using Microsoft.EntityFrameworkCore;
using Quartz;
using TaskFlowMvc.Data;

namespace TaskFlowMvc.Jobs;

[DisallowConcurrentExecution]
public class OverdueTaskCheckJob : IJob
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OverdueTaskCheckJob> _logger;

    public OverdueTaskCheckJob(
        IServiceScopeFactory scopeFactory,
        ILogger<OverdueTaskCheckJob> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        using var timeoutCts = new CancellationTokenSource(TimeSpan.FromMinutes(15));

        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource( context.CancellationToken, timeoutCts.Token);

        try
        {
            using var scope = _scopeFactory.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // TEST ONLY
            //await Task.Delay(
            //    TimeSpan.FromSeconds(2),
            //    linkedCts.Token);

            var thresholdDate = DateTime.UtcNow.AddDays(-7);

            var staleTasks = await dbContext.Task
                .Include(t => t.TaskState)
                .Include(t => t.Project)
                .Where(t => t.TaskState.Name == "In Progress")
                .Where(t => (t.UpdatedAt ?? t.CreatedAt) < thresholdDate)
                .ToListAsync(linkedCts.Token);

            if (staleTasks.Count == 0)
            {
                _logger.LogInformation(
                    "Overdue task check completed: no stale tasks found.");

                return;
            }

            foreach (var task in staleTasks)
            {
                _logger.LogWarning(
                    "Task '{Title}' in project '{Project}' has been In Progress since {LastUpdate} without update.",
                    task.Title,
                    task.Project.Name,
                    task.UpdatedAt ?? task.CreatedAt);
            }

            _logger.LogInformation(
                "Overdue task check completed: {Count} stale task(s) found.",
                staleTasks.Count);
        }
        catch (OperationCanceledException)
       when (timeoutCts.IsCancellationRequested)
        {
            _logger.LogWarning(
                "OverdueTaskCheckJob was cancelled because it exceeded 15 minutes.");
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning(
                "OverdueTaskCheckJob was cancelled by Quartz.");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unexpected error occurred while executing OverdueTaskCheckJob.");
        }
    }
}