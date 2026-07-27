using System.ComponentModel.DataAnnotations;
using TaskFlowMvc.Models;

namespace TaskFlowMvc.Models;

public class TaskItem
{
    public Guid Id { get; set; }
    public Guid TaskStateId { get; set; }
    public Guid ProjectId { get; set; }

    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
    public int Order { get; set; }
    public ICollection<TaskItemTag> TaskItemTags { get; set; } = new List<TaskItemTag>();
    public DateTime CreatedAt { get; set; }

    public Project Project { get; set; } = null!;
    public TaskState TaskState { get; set; } = null!;
}