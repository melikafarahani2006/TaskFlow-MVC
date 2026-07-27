using System.ComponentModel.DataAnnotations;

namespace TaskFlowMvc.Models.DTOs;

public class CreateTaskItemRequest
{
    [Required]
    [Display(Name = "Project Name")]
    public Guid ProjectId { get; set; }

    [Required]
    [Display(Name = "Task State")]
    public Guid TaskStateId { get; set; }

    public List<Guid> TagIds { get; set; } = new();

    [Required]
    [MaxLength(150)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }
}

public class UpdateTaskItemRequest
{
    [Required]
    [Display(Name = "Project Name")]
    public Guid ProjectId { get; set; }

    [Required]
    [Display(Name = "Task State")]
    public Guid TaskStateId { get; set; }

    public List<Guid> TagIds { get; set; } = new();

    [Required]
    [MaxLength(150)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }
}