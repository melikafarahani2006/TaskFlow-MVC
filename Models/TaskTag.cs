using System.ComponentModel.DataAnnotations.Schema;

namespace TaskFlowMvc.Models;

//[Table("TaskItemTag")]
public class TaskTag
{
    public Guid Id { get; set; }
    public Guid TaskId { get; set; }
    public Guid TagId { get; set; }
    public Task Task { get; set; } = null!;
    public Tag Tag { get; set; } = null!;
}