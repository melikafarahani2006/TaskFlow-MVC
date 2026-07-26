namespace TaskFlowMvc.Models;

public class TaskItemTag
{
    public Guid TaskItemId { get; set; }
    public Guid TagId { get; set; }
    public TaskItem TaskItem { get; set; } = null!;
    public Tag Tag { get; set; } = null!;
}