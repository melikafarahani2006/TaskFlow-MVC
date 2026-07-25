using System.ComponentModel.DataAnnotations;

namespace TaskFlowMvc.Models.Workspace;

public class CreateWorkspaceRequest
{
    [Required]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }
}