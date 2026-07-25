using Microsoft.EntityFrameworkCore;
using TaskFlowMvc.Models.Workspace;

namespace TaskFlowMvc.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Workspace> Workspaces => Set<Workspace>();
}