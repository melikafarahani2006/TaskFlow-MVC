using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskFlowMvc.Data;
using TaskFlowMvc.Models;
using TaskFlowMvc.Models.DTOs;

namespace TaskFlowMvc.Controllers.Api;

[ApiController]
[Route("api/")]
public class TagApiController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public TagApiController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/tag
    [HttpGet("tags")]
    public IActionResult GetAll()
    {
        try
        {
            var tags = _context.Tag
                .Include(x => x.TaskTags)
                    .ThenInclude(x => x.Task)
                        .ThenInclude(x => x.Project)
                .Include(x => x.TaskTags)
                    .ThenInclude(x => x.Task)
                        .ThenInclude(x => x.TaskState)
                .Select(x => new TagResponse
                {
                    Id = x.Id,
                    Name = x.Name,
                    Color = x.Color,

                    Tasks = x.TaskTags
                        .Where(tt => !tt.Task.IsDeleted)
                        .Select(tt => new TaskResponse
                        {
                            Id = tt.Task.Id,
                            Title = tt.Task.Title,
                            Description = tt.Task.Description,
                            Order = tt.Task.Order,

                            ProjectId = tt.Task.ProjectId,
                            ProjectName = tt.Task.Project.Name,

                            TaskStateId = tt.Task.TaskStateId,
                            TaskStateName = tt.Task.TaskState.Name,
                        })
                        .ToList()
                })
                .ToList();

            return Ok(tags);
        }
        catch
        {
            return StatusCode(500, "Unable to load tags.");
        }
    }

    // GET: api/tag/{id}
    [HttpGet("tag/{id:guid}")]
    public IActionResult GetById(Guid id)
    {
        try
        {
            var tag = _context.Tag
                .Include(x => x.TaskTags)
                    .ThenInclude(x => x.Task)
                        .ThenInclude(x => x.Project)
                .Include(x => x.TaskTags)
                    .ThenInclude(x => x.Task)
                        .ThenInclude(x => x.TaskState)
                .Where(x => x.Id == id)
                .Select(x => new TagResponse
                {
                    Id = x.Id,
                    Name = x.Name,
                    Color = x.Color,

                    Tasks = x.TaskTags
                        .Where(tt => !tt.Task.IsDeleted)
                        .Select(tt => new TaskResponse
                        {
                            Id = tt.Task.Id,
                            Title = tt.Task.Title,
                            Description = tt.Task.Description,
                            Order = tt.Task.Order,

                            ProjectId = tt.Task.ProjectId,
                            ProjectName = tt.Task.Project.Name,

                            TaskStateId = tt.Task.TaskStateId,
                            TaskStateName = tt.Task.TaskState.Name,

                        })
                        .ToList()
                })
                .FirstOrDefault();

            if (tag == null)
                return NotFound();

            return Ok(tag);
        }
        catch
        {
            return StatusCode(500, "Unable to load tag.");
        }
    }

    // POST: api/tag
    [HttpPost("tag")]
    public IActionResult Create(CreateTagRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var tag = new Tag
            {
                Name = request.Name,
                Color = request.Color
            };

            _context.Tag.Add(tag);
            _context.SaveChanges();

            return CreatedAtAction(
                nameof(GetById),
                new { id = tag.Id },
                tag);
        }
        catch
        {
            return StatusCode(500, "Failed to create tag.");
        }
    }

    // PUT: api/tag/{id}
    [HttpPut("tag/{id:guid}")]
    public IActionResult Update(Guid id, UpdateTagRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var tag = _context.Tag.Find(id);

            if (tag == null)
                return NotFound();

            tag.Name = request.Name;
            tag.Color = request.Color;

            _context.SaveChanges();

            var response = _context.Tag
                .Include(x => x.TaskTags)
                    .ThenInclude(x => x.Task)
                        .ThenInclude(x => x.Project)
                .Include(x => x.TaskTags)
                    .ThenInclude(x => x.Task)
                        .ThenInclude(x => x.TaskState)
                .Where(x => x.Id == id)
                .Select(x => new TagResponse
                {
                    Id = x.Id,
                    Name = x.Name,
                    Color = x.Color,

                    Tasks = x.TaskTags
                        .Where(tt => !tt.Task.IsDeleted)
                        .Select(tt => new TaskResponse
                        {
                            Id = tt.Task.Id,
                            Title = tt.Task.Title,
                            Description = tt.Task.Description,
                            Order = tt.Task.Order,

                            ProjectId = tt.Task.ProjectId,
                            ProjectName = tt.Task.Project.Name,

                            TaskStateId = tt.Task.TaskStateId,
                            TaskStateName = tt.Task.TaskState.Name,

                        })
                        .ToList()
                })
                .FirstOrDefault();

            return Ok(response);
        }
        catch
        {
            return StatusCode(500, "Failed to update tag.");
        }
    }

    // DELETE: api/tag/{id}
    [HttpDelete("tag/{id:guid}")]
    public IActionResult Delete(Guid id)
    {
        try
        {
            var tag = _context.Tag.Find(id);

            if (tag == null)
                return NotFound();

            var isUsed = _context.TaskTag.Any(x => x.TagId == id);

            if (isUsed)
            {
                return BadRequest(
                    "Tag cannot be deleted. Remove it from all tasks first.");
            }

            tag.IsDeleted = true;

            _context.SaveChanges();

            return Ok("Tag deleted successfully.");
        }
        catch
        {
            return StatusCode(500, "Failed to delete tag.");
        }
    }
}