using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskFlowMvc.Data;
using TaskFlowMvc.Models;
using TaskFlowMvc.Models.DTOs;

namespace TaskFlowMvc.Controllers.Api;

[ApiController]
[Route("api/")]
public class TaskStateApiController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public TaskStateApiController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("taskStates")]
    public IActionResult GetAll()
    {
        try
        {
            var taskStates = _context.TaskState
                .Include(x => x.Tasks)
                    .ThenInclude(x => x.Project)
                .Select(x => new TaskStateResponse
                {
                    Id = x.Id,
                    Name = x.Name,

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
                            ProjectName = t.Project.Name,

                            TaskStateId = t.TaskStateId,
                            TaskStateName = x.Name,

                        })
                        .ToList()
                })
                .ToList();

            return Ok(taskStates);
        }
        catch
        {
            return StatusCode(500, "Unable to load task states.");
        }
    }

    [HttpGet("taskState/{id:guid}")]
    public IActionResult GetById(Guid id)
    {
        try
        {
            var taskState = _context.TaskState
                .Include(x => x.Tasks)
                    .ThenInclude(x => x.Project)
                .Where(x => x.Id == id)
                .Select(x => new TaskStateResponse
                {
                    Id = x.Id,
                    Name = x.Name,

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
                            ProjectName = t.Project.Name,

                            TaskStateId = t.TaskStateId,
                            TaskStateName = x.Name,

                        })
                        .ToList()
                })
                .FirstOrDefault();

            if (taskState == null)
                return NotFound();

            return Ok(taskState);
        }
        catch
        {
            return StatusCode(500, "Unable to load task state.");
        }
    }

    // POST: api/taskstate
    [HttpPost("taskState")]
    public IActionResult Create(CreateTaskStateRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var taskState = new TaskState
            {
                Name = request.Name
            };

            _context.TaskState.Add(taskState);
            _context.SaveChanges();

            return CreatedAtAction(
                nameof(GetById),
                new { id = taskState.Id },
                taskState);
        }
        catch
        {
            return StatusCode(500, "Failed to create task state.");
        }
    }

    [HttpPut("taskState/{id:guid}")]
    public IActionResult Update(Guid id, UpdateTaskStateRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var taskState = _context.TaskState.Find(id);

            if (taskState == null)
                return NotFound();

            taskState.Name = request.Name;

            _context.SaveChanges();

            var response = _context.TaskState
                .Include(x => x.Tasks)
                    .ThenInclude(x => x.Project)
                .Where(x => x.Id == id)
                .Select(x => new TaskStateResponse
                {
                    Id = x.Id,
                    Name = x.Name,

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
                            ProjectName = t.Project.Name,

                            TaskStateId = t.TaskStateId,
                            TaskStateName = x.Name,

                        })
                        .ToList()
                })
                .FirstOrDefault();

            return Ok(response);
        }
        catch
        {
            return StatusCode(500, "Failed to update task state.");
        }
    }

    // DELETE: api/taskstate/{id}
    [HttpDelete("taskState/{id:guid}")]
    public IActionResult Delete(Guid id)
    {
        try
        {
            var taskState = _context.TaskState.Find(id);

            if (taskState == null)
                return NotFound();

            var hasTasks = _context.Task.Any(x => x.TaskStateId == id);

            if (hasTasks)
            {
                return BadRequest(
                    "Task state cannot be deleted. Move the tasks to another state or delete them first.");
            }

            taskState.IsDeleted = true;

            _context.SaveChanges();

            return Ok("Task state deleted successfully.");
        }
        catch
        {
            return StatusCode(500, "Failed to delete task state.");
        }
    }
}