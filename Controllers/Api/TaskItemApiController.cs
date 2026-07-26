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

    [HttpGet("taskItems")]
    public IActionResult GetAll()
    {
        return Ok(
            _context.TaskItem
                //.Include(x => x.Project)
                //.Include(x => x.TaskState)
                .ToList());
    }

    [HttpGet("taskItem/{id}")]
    public IActionResult Get(Guid id)
    {
        var task = _context.TaskItem
            //.Include(x => x.Project)
            //.Include(x => x.TaskState)
            .FirstOrDefault(x => x.Id == id);

        if (task == null)
            return NotFound();

        return Ok(task);
    }

    [HttpPost("taskItem")]
    public IActionResult Create(CreateTaskItemRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var task = new TaskItem
        {
            ProjectId = request.ProjectId,
            TaskStateId = request.TaskStateId,
            Title = request.Title,
            Description = request.Description,
            Order = request.Order,
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow
        };

        _context.TaskItem.Add(task);
        _context.SaveChanges();

        return CreatedAtAction(nameof(Get), new { id = task.Id }, task);
    }

    [HttpPut("taskItem/{id}")]
    public IActionResult Update(Guid id, UpdateTaskItemRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var task = _context.TaskItem.Find(id);

        if (task == null)
            return NotFound();

        task.ProjectId = request.ProjectId;
        task.TaskStateId = request.TaskStateId;
        task.Title = request.Title;
        task.Description = request.Description;
        task.IsCompleted = request.IsCompleted;
        task.Order = request.Order;

        _context.SaveChanges();

        return NoContent();
    }

    [HttpDelete("taskItem/{id}")]
    public IActionResult Delete(Guid id)
    {
        var task = _context.TaskItem.Find(id);

        if (task == null)
            return NotFound();

        _context.TaskItem.Remove(task);
        _context.SaveChanges();

        return NoContent();
    }
}