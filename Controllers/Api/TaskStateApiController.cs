using Microsoft.AspNetCore.Mvc;
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
        return Ok(_context.TaskState.ToList());
    }

    [HttpGet("taskState/{id}")]
    public IActionResult Get(Guid id)
    {
        var taskState = _context.TaskState.Find(id);

        if (taskState == null)
            return NotFound();

        return Ok(taskState);
    }

    [HttpPost("taskState")]
    public IActionResult Create(CreateTaskStateRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var taskState = new TaskState
        {
            Name = request.Name
        };

        _context.TaskState.Add(taskState);
        _context.SaveChanges();

        return CreatedAtAction(nameof(Get), new { id = taskState.Id }, taskState);
    }

    [HttpPut("taskState/{id}")]
    public IActionResult Update(Guid id, UpdateTaskStateRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var taskState = _context.TaskState.Find(id);

        if (taskState == null)
            return NotFound();

        taskState.Name = request.Name;

        _context.SaveChanges();

        return NoContent();
    }

    [HttpDelete("taskState/{id}")]
    public IActionResult Delete(Guid id)
    {
        var taskState = _context.TaskState.Find(id);

        if (taskState == null)
            return NotFound();

        _context.TaskState.Remove(taskState);
        _context.SaveChanges();

        return NoContent();
    }
}