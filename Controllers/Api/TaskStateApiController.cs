using Microsoft.AspNetCore.Mvc;
using TaskFlowMvc.Data;
using TaskFlowMvc.Models;
using TaskFlowMvc.Models.DTOs;

namespace TaskFlowMvc.Controllers.Api;

[ApiController]
[Route("api/")]
public class TaskStateController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public TaskStateController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/taskstate
    [HttpGet("taskStates")]
    public IActionResult GetAll()
    {
        try
        {
            var taskStates = _context.TaskState.ToList();
            return Ok(taskStates);
        }
        catch
        {
            return StatusCode(500, "Unable to load task states.");
        }
    }

    // GET: api/taskstate/{id}
    [HttpGet("taskState/{id:guid}")]
    public IActionResult GetById(Guid id)
    {
        try
        {
            var taskState = _context.TaskState.Find(id);

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

    // PUT: api/taskstate/{id}
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

            return Ok(taskState);
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