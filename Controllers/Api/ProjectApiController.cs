using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskFlowMvc.Data;
using TaskFlowMvc.Models.DTOs;
using TaskFlowMvc.Models;

namespace TaskFlowMvc.Controllers.Api;

[ApiController]
[Route("api/")]
public class ProjectApiController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ProjectApiController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/projects
    [HttpGet("projects")]
    public async Task<IActionResult> GetAll()
    {
        var projects = await _context.Project
            .ToListAsync();

        return Ok(projects);
    }

    // GET: api/projects/{id}
    [HttpGet("project/{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var project = await _context.Project
            .FirstOrDefaultAsync(x => x.Id == id);

        if (project == null)
            return NotFound();

        return Ok(project);
    }

    // POST: api/projects
    [HttpPost("project/{workspaceId:guid}")]
    public async Task<IActionResult> Create(Guid workspaceId, CreateProjectRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var workspaceExists = await _context.Workspace
            .AnyAsync(x => x.Id == workspaceId);

        if (!workspaceExists)
            return NotFound("Workspace not found.");

        var project = new Project
        {
            WorkspaceId = workspaceId,
            Name = request.Name,
            Description = request.Description,
            CreatedAt = DateTime.UtcNow
        };

        _context.Project.Add(project);

        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetById),
            new { id = project.Id },
            project);
    }

    [HttpPatch("project/{id:guid}")]
    public async Task<IActionResult> Update(
    Guid id,
    UpdateProjectRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var project = await _context.Project.FindAsync(id);

        if (project == null)
            return NotFound();

        project.Name = request.Name;
        project.Description = request.Description;

        await _context.SaveChangesAsync();

        return Ok(project);
    }

    [HttpDelete("project/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var project = await _context.Project.FindAsync(id);

        if (project == null)
            return NotFound();

        _context.Project.Remove(project);

        await _context.SaveChangesAsync();

        return Ok("Deleted successfully");
    }
}