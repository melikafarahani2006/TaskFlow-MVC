using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskFlowMvc.Data;
using TaskFlowMvc.Models.Workspace;

namespace TaskFlowMvc.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class WorkspaceApiController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public WorkspaceApiController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/workspaceapi
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Workspace>>> GetAll()
    {
        var workspaces = await _context.Workspaces.ToListAsync();

        return Ok(workspaces);
    }

    // GET: api/workspaceapi/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Workspace>> GetById(Guid id)
    {
        var workspace = await _context.Workspaces.FindAsync(id);

        if (workspace == null)
            return NotFound();

        return Ok(workspace);
    }

    // POST: api/workspaceapi
    [HttpPost]
    public async Task<ActionResult> Create(CreateWorkspaceRequest request)
    {
        var workspace = new Workspace
        {
            Name = request.Name,
            Description = request.Description,
            CreatedAt = DateTime.UtcNow
        };

        _context.Workspaces.Add(workspace);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = workspace.Id }, workspace);
    }

    // PUT: api/workspaceapi/{id}
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, Workspace model)
    {
        var workspace = await _context.Workspaces.FindAsync(id);

        if (workspace == null)
            return NotFound();

        workspace.Name = model.Name;
        workspace.Description = model.Description;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/workspaceapi/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var workspace = await _context.Workspaces.FindAsync(id);

        if (workspace == null)
            return NotFound();

        _context.Workspaces.Remove(workspace);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}