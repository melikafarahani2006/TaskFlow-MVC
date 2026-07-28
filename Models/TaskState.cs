using System.ComponentModel.DataAnnotations;
using TaskFlowMvc.Models;

namespace TaskFlowMvc.Models;

public class TaskState : BaseEntity
{
    public Guid Id { get; set; }

    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    public int Order { get; set; }
    public ICollection<Task> Tasks { get; set; } = new List<Task>();
}