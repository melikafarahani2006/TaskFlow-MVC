using TaskFlowMvc.Models;
using TaskFlowMvc.Models.DTOs;

public class ProjectResponse : BaseEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid WorkspaceId { get; set; }
    public string WorkspaceName { get; set; } = string.Empty;
    public List<TaskResponse> Tasks { get; set; } = [];
}