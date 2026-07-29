using Microsoft.EntityFrameworkCore;
using TaskFlowMvc.Data;
using TaskFlowMvc.Models;
using TaskFlowMvc.Models.DTOs;

namespace TaskFlowMvc.Services;

public class WorkspaceService
{
    private readonly ApplicationDbContext _context;

    public WorkspaceService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async System.Threading.Tasks.Task CreateWorkspaceWithDefaultProject(CreateWorkspaceRequest request)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var workspace = new Workspace
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description
            };

            _context.Workspace.Add(workspace);

            await _context.SaveChangesAsync();

            var project = new Project
            {
                Id = Guid.NewGuid(),
                WorkspaceId = workspace.Id,
                Name = "Default Project"
            };

            _context.Project.Add(project);

            await _context.SaveChangesAsync();

            //throw new Exception("Test Rollback");

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}