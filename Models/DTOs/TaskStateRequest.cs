using System.ComponentModel.DataAnnotations;

namespace TaskFlowMvc.Models.DTOs;

public class CreateTaskStateRequest
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    public int Order { get; set; }
}

public class UpdateTaskStateRequest
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    public int Order { get; set; }
}