using System.ComponentModel.DataAnnotations;

namespace TaskFlowMvc.Models;

public class Workspace
{
    public Guid Id { get; set; }

    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }

    public ICollection<Project> Projects { get; set; } = [];
}