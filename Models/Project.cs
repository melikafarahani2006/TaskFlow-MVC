using System.ComponentModel.DataAnnotations;

namespace TaskFlowMvc.Models;

public class Project
{
    public Guid Id { get; set; }
    public Guid WorkspaceId { get; set; }

    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }

    public Workspace? Workspace { get; set; }
}