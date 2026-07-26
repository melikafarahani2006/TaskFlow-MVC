namespace TaskFlowMvc.Models;

public class Tag
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Color { get; set; }
    public ICollection<TaskItemTag> TaskItemTags { get; set; } = new List<TaskItemTag>();
}