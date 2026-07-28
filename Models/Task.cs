using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TaskFlowMvc.Models;

namespace TaskFlowMvc.Models;

//[Table("TaskItem")]
public class Task : BaseEntity
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
    public ICollection<TaskTag> TaskTags { get; set; } = new List<TaskTag>();
    public Project Project { get; set; } = null!;
    public TaskState TaskState { get; set; } = null!;
}