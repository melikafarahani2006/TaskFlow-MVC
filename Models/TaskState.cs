using System.ComponentModel.DataAnnotations;
using TaskFlowMvc.Models;

namespace TaskFlowMvc.Models;

public class TaskState
{
    public Guid Id { get; set; }

    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    public int Order { get; set; }
    public DateTime CreatedAt { get; set; }
    public ICollection<TaskItem> TaskItems { get; set; } = new List<TaskItem>();
}