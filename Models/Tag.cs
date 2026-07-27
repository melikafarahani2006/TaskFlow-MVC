using System.ComponentModel.DataAnnotations;

namespace TaskFlowMvc.Models;

public class Tag
{
    public Guid Id { get; set; }

    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? Color { get; set; }
    public ICollection<TaskItemTag> TaskItemTags { get; set; } = new List<TaskItemTag>();
}