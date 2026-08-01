using System.ComponentModel.DataAnnotations;

namespace TaskFlowMvc.Models;

public class Workspace :BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Description { get; set; }
    public ICollection<Project> Projects { get; set; } = [];
    public ICollection<WorkspaceMember> Members { get; set; } = new List<WorkspaceMember>();
}