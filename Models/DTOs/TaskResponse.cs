namespace TaskFlowMvc.Models.DTOs;

public class TaskResponse :BaseEntity
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
    public int Order { get; set; }
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public Guid TaskStateId { get; set; }
    public string TaskStateName { get; set; } = string.Empty;
    public List<Tag> Tags { get; set; } = [];
    public string TagsText { get; set; } = "";
    public string CreatedAtText => CreatedAt.ToString("yyyy/MM/dd HH:mm") ?? "";
}