using Microsoft.AspNetCore.Mvc;
using TaskFlowMvc.Data;
using TaskFlowMvc.Models;
using TaskFlowMvc.Models.DTOs;

namespace TaskFlowMvc.Controllers.Api;

[ApiController]
[Route("api/")]
public class TagController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public TagController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/tag
    [HttpGet("tags")]
    public IActionResult GetAll()
    {
        try
        {
            var tags = _context.Tag.ToList();
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
            var tag = _context.Tag.Find(id);

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

            return Ok(tag);
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