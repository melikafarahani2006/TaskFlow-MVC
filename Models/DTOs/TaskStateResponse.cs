using TaskFlowMvc.Models.DTOs;

public class TaskStateResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<TaskResponse> Tasks { get; set; } = [];
}