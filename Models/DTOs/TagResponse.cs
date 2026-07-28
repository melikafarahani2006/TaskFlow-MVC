using TaskFlowMvc.Models.DTOs;

public class TagResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Color { get; set; }
    public List<TaskResponse> Tasks { get; set; } = [];
}