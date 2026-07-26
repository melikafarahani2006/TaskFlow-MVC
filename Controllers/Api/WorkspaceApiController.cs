using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskFlowMvc.Data;
using TaskFlowMvc.Models.DTOs;
using TaskFlowMvc.Models;

namespace TaskFlowMvc.Controllers.Api;

[ApiController]
[Route("api/")]
public class WorkspaceApiController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public WorkspaceApiController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/workspaces
    [HttpGet("workspaces")]
    public async Task<ActionResult<IEnumerable<Workspace>>> GetAll()
    {
        var workspaces = await _context.Workspace.ToListAsync();

        return Ok(workspaces);
    }

    // GET: api/workspace/{id}
    [HttpGet("workspace/{id:guid}")]
    public async Task<ActionResult<Workspace>> GetById(Guid id)
    {
        var workspace = await _context.Workspace.FindAsync(id);

        if (workspace == null)
            return NotFound();

        return Ok(workspace);
    }

    // POST: api/workspace
    [HttpPost("workspace")]
    public async Task<ActionResult> Create(CreateWorkspaceRequest request)
    {
        var workspace = new Workspace
        {
            Name = request.Name,
            Description = request.Description,
            CreatedAt = DateTime.UtcNow
        };

        _context.Workspace.Add(workspace);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = workspace.Id }, workspace);
    }

    // PUT: api/workspace/{id}
    [HttpPatch("workspace/{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateWorkspaceRequest request)
    {
        var workspace = await _context.Workspace.FindAsync(id);

        if (workspace == null)
            return NotFound();

        workspace.Name = request.Name;
        workspace.Description = request.Description;

        await _context.SaveChangesAsync();

        return Ok(workspace);
    }

    // DELETE: api/workspace/{id}
    [HttpDelete("workspace/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var workspace = await _context.Workspace.FindAsync(id);

        if (workspace == null)
            return NotFound();

        _context.Workspace.Remove(workspace);
        await _context.SaveChangesAsync();

        return Ok("Deleted successfully");
    }
}