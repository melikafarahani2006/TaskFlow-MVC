using Microsoft.AspNetCore.Identity;

namespace TaskFlowMvc.Models;

public class WorkspaceMember : BaseEntity
{
    public Guid Id { get; set; }

    public Guid WorkspaceId { get; set; }
    public Workspace Workspace { get; set; } = null!;

    public string UserId { get; set; } = null!;
    public IdentityUser User { get; set; } = null!;

    public string Role { get; set; } = "Member"; // e.g. "Owner", "Member"
}