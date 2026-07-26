using Microsoft.AspNetCore.Mvc;
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

    [HttpGet("tags")]
    public IActionResult GetAll()
    {
        return Ok(_context.Tag.ToList());
    }

    [HttpGet("tag/{id}")]
    public IActionResult Get(Guid id)
    {
        var tag = _context.Tag.Find(id);

        if (tag == null)
            return NotFound();

        return Ok(tag);
    }

    [HttpPost("tag")]
    public IActionResult Create(CreateTagRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var tag = new Tag
        {
            Name = request.Name,
            Color = request.Color
        };

        _context.Tag.Add(tag);
        _context.SaveChanges();

        return CreatedAtAction(nameof(Get), new { id = tag.Id }, tag);
    }

    [HttpPut("tag/{id}")]
    public IActionResult Update(Guid id, UpdateTagRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var tag = _context.Tag.Find(id);

        if (tag == null)
            return NotFound();

        tag.Name = request.Name;
        tag.Color = request.Color;

        _context.SaveChanges();

        return NoContent();
    }

    [HttpDelete("tag/{id}")]
    public IActionResult Delete(Guid id)
    {
        var tag = _context.Tag.Find(id);

        if (tag == null)
            return NotFound();

        _context.Tag.Remove(tag);
        _context.SaveChanges();

        return NoContent();
    }
}