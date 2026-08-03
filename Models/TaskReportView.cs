using System.ComponentModel.DataAnnotations.Schema;

namespace TaskFlowMvc.Models;

public class TaskReportView
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
    public int Order { get; set; }
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public Guid WorkspaceId { get; set; }
    public Guid TaskStateId { get; set; }
    public string TaskStateName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? TagsText { get; set; }

    [NotMapped]
    public string CreatedAtShamsi { get; set; } = string.Empty;

    [NotMapped]
    public string DueDateShamsi { get; set; } = string.Empty;

    [NotMapped]
    public string UpdatedAtShamsi { get; set; } = string.Empty;
}