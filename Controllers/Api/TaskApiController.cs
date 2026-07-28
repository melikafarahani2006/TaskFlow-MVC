using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskFlowMvc.Data;
using TaskFlowMvc.Models;
using TaskFlowMvc.Models.DTOs;

namespace TaskFlowMvc.Controllers.Api;

[ApiController]
[Route("api/")]
public class TaskController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<TaskController> _logger;

    public TaskController(ApplicationDbContext context, ILogger<TaskController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: api/task
    [HttpGet("tasks")]
    public IActionResult GetAll()
    {
        try
        {
            var tasks = _context.Task
                .Include(x => x.Project)
                .Include(x => x.TaskState)
                .Include(x => x.TaskTags)
                .ThenInclude(x => x.Tag)
                .Select(x => new TaskResponse
                {
                    Id = x.Id,
                    Title = x.Title,
                    Description = x.Description,
                    DueDate = x.DueDate,
                    Order = x.Order,

                    ProjectId = x.ProjectId,
                    ProjectName = x.Project.Name,

                    TaskStateId = x.TaskStateId,
                    TaskStateName = x.TaskState.Name,

                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,

                    Tags = x.TaskTags
                        .Select(t => new Tag
                        {
                            Id = t.Tag.Id,
                            Name = t.Tag.Name,
                            Color = t.Tag.Color,
                            CreatedAt = t.Tag.CreatedAt,
                            UpdatedAt = t.Tag.UpdatedAt
                        })
                        .ToList()
                })
                .ToList();

            return Ok(tasks);
        }
        catch
        {
            return StatusCode(500, "Unable to load tasks.");
        }
    }

    // GET: api/task/{id}
    [HttpGet("task/{id:guid}")]
    public IActionResult GetById(Guid id)
    {
        try
        {
            var task = _context.Task
                .Include(x => x.Project)
                .Include(x => x.TaskState)
                .Include(x => x.TaskTags)
                .ThenInclude(x => x.Tag)
                .Where(x => x.Id == id)
                .Select(x => new TaskResponse
                {
                    Id = x.Id,
                    Title = x.Title,
                    Description = x.Description,
                    DueDate = x.DueDate,
                    Order = x.Order,

                    ProjectId = x.ProjectId,
                    ProjectName = x.Project.Name,

                    TaskStateId = x.TaskStateId,
                    TaskStateName = x.TaskState.Name,

                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,

                    Tags = x.TaskTags
                        .Select(t => new Tag
                        {
                            Id = t.Tag.Id,
                            Name = t.Tag.Name,
                            Color = t.Tag.Color,
                            CreatedAt = t.Tag.CreatedAt,
                            UpdatedAt = t.Tag.UpdatedAt
                        })
                        .ToList()
                })
                .FirstOrDefault();

            if (task == null)
                return NotFound();

            return Ok(task);
        }
        catch
        {
            return StatusCode(500, "Unable to load task.");
        }
    }

    // POST: api/task
    [HttpPost("task")]
    public IActionResult Create(CreateTaskRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var maxOrder = _context.Task
                .Where(x => x.ProjectId == request.ProjectId)
                .Select(x => (int?)x.Order)
                .Max() ?? 0;

            var task = new TaskFlowMvc.Models.Task
            {
                ProjectId = request.ProjectId,
                TaskStateId = request.TaskStateId,
                Title = request.Title,
                Description = request.Description,
                Order = maxOrder + 1
            };

            _context.Task.Add(task);
            _context.SaveChanges();

            foreach (var tagId in request.TagIds)
            {
                _context.TaskTag.Add(new TaskTag
                {
                    TaskId = task.Id,
                    TagId = tagId
                });
            }

            _context.SaveChanges();

            return CreatedAtAction(
                nameof(GetById),
                new { id = task.Id },
                task);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while creating task.");

            return StatusCode(500, "Failed to create task.");
        }
    }

    // PUT: api/task/{id}
    [HttpPut("task/{id:guid}")]
    public IActionResult Update(Guid id, UpdateTaskRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var task = _context.Task.Find(id);

            if (task == null)
                return NotFound();

            task.ProjectId = request.ProjectId;
            task.TaskStateId = request.TaskStateId;
            task.Title = request.Title;
            task.Description = request.Description;

            var oldTags = _context.TaskTag
                .Where(x => x.TaskId == id);

            _context.TaskTag.RemoveRange(oldTags);

            foreach (var tagId in request.TagIds)
            {
                _context.TaskTag.Add(new TaskTag
                {
                    TaskId = id,
                    TagId = tagId
                });
            }

            _context.SaveChanges();

            return Ok(task);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while updating task.");

            return StatusCode(500, "Failed to update task.");
        }
    }

    // DELETE: api/task/{id}
    [HttpDelete("task/{id:guid}")]
    public IActionResult Delete(Guid id)
    {
        try
        {
            var task = _context.Task.Find(id);

            if (task == null)
                return NotFound();

            task.IsDeleted = true;

            _context.SaveChanges();

            return Ok("Task deleted successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while deleting task.");

            return StatusCode(500, "Failed to delete task.");
        }
    }
}