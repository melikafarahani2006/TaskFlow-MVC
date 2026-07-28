using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskFlowMvc.Data;
using TaskFlowMvc.Models;
using TaskFlowMvc.Models.DTOs;

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

    // GET: api/workspace
    [HttpGet("workspaces")]
    public IActionResult GetAll()
    {
        try
        {
            var workspaces = _context.Workspace
                 .Include(x => x.Projects)
                 .Select(x => new
            {
                     x.Id,
                     x.Name,
                     x.Description,

                     Projects = x.Projects
                    .Where(p => !p.IsDeleted)
                    .Select(p => new ProjectResponse
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        WorkspaceId = p.WorkspaceId,
                        WorkspaceName = x.Name,
                    })
                    .ToList()
            })
            .ToList();
            return Ok(workspaces);
        }
        catch
        {
            return StatusCode(500, "Unable to load workspaces.");
        }
    }

    // GET: api/workspace/{id}
    [HttpGet("workspace/{id:guid}")]
    public IActionResult GetById(Guid id)
    {
        try
        {
            var workspace = _context.Workspace
                       .Include(x => x.Projects)
                       .Where(x => x.Id == id)
                       .Select(x => new
                       {
                           x.Id,
                           x.Name,
                           x.Description,

                           Projects = x.Projects
                               .Where(p => !p.IsDeleted)
                               .Select(p => new ProjectResponse
                               {
                                   Id = p.Id,
                                   Name = p.Name,
                                   Description = p.Description,
                                   WorkspaceId = p.WorkspaceId,
                                   WorkspaceName = x.Name,
                               })
                               .ToList()
                       })
                       .FirstOrDefault();
            if (workspace == null)
                return NotFound();

            return Ok(workspace);
        }
        catch
        {
            return StatusCode(500, "Unable to load workspace.");
        }
    }

    // POST: api/workspace
    [HttpPost("workspace")]
    public IActionResult Create(CreateWorkspaceRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var workspace = new Workspace
            {
                Name = request.Name,
                Description = request.Description
            };

            _context.Workspace.Add(workspace);
            _context.SaveChanges();

            return CreatedAtAction(
                nameof(GetById),
                new { id = workspace.Id },
                workspace);
        }
        catch
        {
            return StatusCode(500, "Failed to create workspace.");
        }
    }

    // PUT: api/workspace/{id}
    [HttpPut("workspace/{id:guid}")]
    public IActionResult Update(Guid id, UpdateWorkspaceRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var workspace = _context.Workspace.Find(id);

            if (workspace == null)
                return NotFound();

            workspace.Name = request.Name;
            workspace.Description = request.Description;

            _context.SaveChanges();

            var response = _context.Workspace
                .Include(x => x.Projects)
                .Where(x => x.Id == id)
                .Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.Description,

                    Projects = x.Projects
                        .Where(p => !p.IsDeleted)
                        .Select(p => new ProjectResponse
                        {
                            Id = p.Id,
                            Name = p.Name,
                            Description = p.Description,
                            WorkspaceId = p.WorkspaceId,
                            WorkspaceName = x.Name,
                        })
                        .ToList()
                })
                .FirstOrDefault();

            return Ok(response);
        }
        catch
        {
            return StatusCode(500, "Failed to update workspace.");
        }
    }

    // DELETE: api/workspace/{id}
    [HttpDelete("workspace/{id:guid}")]
    public IActionResult Delete(Guid id)
    {
        try
        {
            var workspace = _context.Workspace.Find(id);

            if (workspace == null)
                return NotFound();

            var hasProjects = _context.Project
                .Any(x => x.WorkspaceId == id);

            if (hasProjects)
            {
                return BadRequest(
                    "Workspace cannot be deleted. Move the projects to another workspace or delete them first.");
            }

            workspace.IsDeleted = true;

            _context.SaveChanges();

            return Ok("Workspace deleted successfully.");
        }
        catch (DbUpdateException)
        {
            return StatusCode(500, "Failed to delete workspace.");
        }
    }
}