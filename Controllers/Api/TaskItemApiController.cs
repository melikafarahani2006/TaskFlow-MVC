using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskFlowMvc.Data;
using TaskFlowMvc.Models;
using TaskFlowMvc.Models.DTOs;

namespace TaskFlowMvc.Controllers.Api;

[ApiController]
[Route("api/")]
public class TaskItemApiController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public TaskItemApiController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("tasks")]
    public IActionResult GetAll()
    {
        return Ok(
            _context.Task
                //.Include(x => x.Project)
                //.Include(x => x.TaskState)
                .ToList());
    }

    [HttpGet("task/{id}")]
    public IActionResult Get(Guid id)
    {
        var task = _context.Task
            //.Include(x => x.Project)
            //.Include(x => x.TaskState)
            .FirstOrDefault(x => x.Id == id);

        if (task == null)
            return NotFound();

        return Ok(task);
    }

    [HttpPost("task")]
    public IActionResult Create(CreateTaskRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var task = new Models.Task
        {
            ProjectId = request.ProjectId,
            TaskStateId = request.TaskStateId,
            Title = request.Title,
            Description = request.Description,
            CreatedAt = DateTime.UtcNow
        };

        _context.Task.Add(task);
        _context.SaveChanges();

        return CreatedAtAction(nameof(Get), new { id = task.Id }, task);
    }

    [HttpPut("task/{id}")]
    public IActionResult Update(Guid id, UpdateTaskRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var task = _context.Task.Find(id);

        if (task == null)
            return NotFound();

        task.ProjectId = request.ProjectId;
        task.TaskStateId = request.TaskStateId;
        task.Title = request.Title;
        task.Description = request.Description;

        _context.SaveChanges();

        return NoContent();
    }

    [HttpDelete("task/{id}")]
    public IActionResult Delete(Guid id)
    {
        var task = _context.Task.Find(id);

        if (task == null)
            return NotFound();

        _context.Task.Remove(task);
        _context.SaveChanges();

        return NoContent();
    }
}