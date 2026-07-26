using System.ComponentModel.DataAnnotations;

namespace TaskFlowMvc.Models.DTOs;

public class CreateTaskItemRequest
{
    [Required]
    public Guid ProjectId { get; set; }

    [Required]
    public Guid TaskStateId { get; set; }

    public List<Guid> TagIds { get; set; } = new();

    [Required]
    [MaxLength(150)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }
    public int Order { get; set; }
}

public class UpdateTaskItemRequest
{
    [Required]
    public Guid ProjectId { get; set; }

    [Required]
    public Guid TaskStateId { get; set; }

    public List<Guid> TagIds { get; set; } = new();

    [Required]
    [MaxLength(150)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }
    public bool IsCompleted { get; set; }
    public int Order { get; set; }
}