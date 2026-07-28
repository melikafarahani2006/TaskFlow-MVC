using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskFlowMvc.Data;
using TaskFlowMvc.Models;
using TaskFlowMvc.Models.DTOs;

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
    public IActionResult GetAll()
    {
        try
        {
            var projects = _context.Project
                .Include(x => x.Workspace)
                .Include(x => x.Tasks)
                    .ThenInclude(x => x.TaskState)
                .Select(x => new ProjectResponse
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    WorkspaceId = x.WorkspaceId,
                    WorkspaceName = x.Workspace!.Name,

                    Tasks = x.Tasks
                        .Where(t => !t.IsDeleted)
                        .Select(t => new TaskResponse
                        {
                            Id = t.Id,
                            Title = t.Title,
                            TaskStateName = t.TaskState.Name
                        })
                        .ToList()
                })
                .ToList();

            return Ok(projects);
        }
        catch
        {
            return StatusCode(500, "Unable to load projects.");
        }
    }

    // GET: api/project/{id}
    [HttpGet("project/{id:guid}")]
    public IActionResult GetById(Guid id)
    {
        try
        {
            var project = _context.Project
                .Include(x => x.Workspace)
                .Include(x => x.Tasks)
                    .ThenInclude(x => x.TaskState)
                .Where(x => x.Id == id)
                .Select(x => new ProjectResponse
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    WorkspaceId = x.WorkspaceId,
                    WorkspaceName = x.Workspace!.Name,

                    Tasks = x.Tasks
                        .Where(t => !t.IsDeleted)
                        .Select(t => new TaskResponse
                        {
                            Id = t.Id,
                            Title = t.Title,
                            TaskStateName = t.TaskState.Name
                        })
                        .ToList()
                })
                .FirstOrDefault();

            if (project == null)
                return NotFound();

            return Ok(project);
        }
        catch
        {
            return StatusCode(500, "Unable to load project.");
        }
    }

    // POST: api/project
    [HttpPost("project")]
    public IActionResult Create(CreateProjectRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var workspaceExists = _context.Workspace
                .Any(x => x.Id == request.WorkspaceId);

            if (!workspaceExists)
                return BadRequest("Workspace not found.");

            var project = new Project
            {
                WorkspaceId = request.WorkspaceId,
                Name = request.Name,
                Description = request.Description
            };

            _context.Project.Add(project);
            _context.SaveChanges();

            return CreatedAtAction(
                nameof(GetById),
                new { id = project.Id },
                project);
        }
        catch
        {
            return StatusCode(500, "Failed to create project.");
        }
    }

    // PUT: api/project/{id}
    [HttpPut("project/{id:guid}")]
    public IActionResult Update(Guid id, UpdateProjectRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var project = _context.Project.Find(id);

            if (project == null)
                return NotFound();

            project.Name = request.Name;
            project.Description = request.Description;

            _context.SaveChanges();

            var response = _context.Project
                .Include(x => x.Workspace)
                .Include(x => x.Tasks)
                    .ThenInclude(x => x.TaskState)
                .Where(x => x.Id == id)
                .Select(x => new ProjectResponse
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,

                    WorkspaceId = x.WorkspaceId,
                    WorkspaceName = x.Workspace!.Name,

                    Tasks = x.Tasks
                        .Where(t => !t.IsDeleted)
                        .Select(t => new TaskResponse
                        {
                            Id = t.Id,
                            Title = t.Title,
                            Description = t.Description,
                            DueDate = t.DueDate,
                            Order = t.Order,

                            ProjectId = t.ProjectId,
                            ProjectName = x.Name,

                            TaskStateId = t.TaskStateId,
                            TaskStateName = t.TaskState.Name,

                        })
                        .ToList()
                })
                .FirstOrDefault();

            return Ok(response);
        }
        catch
        {
            return StatusCode(500, "Failed to update project.");
        }
    }

    // DELETE: api/project/{id}
    [HttpDelete("project/{id:guid}")]
    public IActionResult Delete(Guid id)
    {
        try
        {
            var project = _context.Project.Find(id);

            if (project == null)
                return NotFound();

            var hasTasks = _context.Task.Any(x => x.ProjectId == id);

            if (hasTasks)
            {
                return BadRequest(
                    "Project cannot be deleted. Move the tasks to another project or delete them first.");
            }

            project.IsDeleted = true;

            _context.SaveChanges();

            return Ok("Project deleted successfully.");
        }
        catch
        {
            return StatusCode(500, "Failed to delete project.");
        }
    }
}