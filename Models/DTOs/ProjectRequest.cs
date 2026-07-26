using System.ComponentModel.DataAnnotations;

namespace TaskFlowMvc.Models.DTOs;

public class CreateProjectRequest
{
    [Required]
    public Guid WorkspaceId { get; set; }

    [Required(ErrorMessage = "Name is required.")]
    [MaxLength(100, ErrorMessage = "Maximum 100 characters.")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }
}

public class UpdateProjectRequest
{
    [Required(ErrorMessage = "Name is required.")]
    [MaxLength(100, ErrorMessage = "Maximum 100 characters.")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }
}