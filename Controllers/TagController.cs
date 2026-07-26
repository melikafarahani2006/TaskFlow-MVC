using Microsoft.AspNetCore.Mvc;
using TaskFlowMvc.Data;
using TaskFlowMvc.Models;
using TaskFlowMvc.Models.DTOs;

namespace TaskFlowMvc.Controllers;

public class TagController : Controller
{
    private readonly ApplicationDbContext _context;

    public TagController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        return View(_context.Tag.ToList());
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(CreateTagRequest request)
    {
        if (!ModelState.IsValid)
            return View(request);

        var tag = new Tag
        {
            Name = request.Name,
            Color = request.Color
        };

        _context.Tag.Add(tag);
        _context.SaveChanges();

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Edit(Guid id)
    {
        var tag = _context.Tag.Find(id);

        if (tag == null)
            return NotFound();

        return View(new UpdateTagRequest
        {
            Name = tag.Name,
            Color = tag.Color
        });
    }

    [HttpPost]
    public IActionResult Edit(Guid id, UpdateTagRequest request)
    {
        if (!ModelState.IsValid)
            return View(request);

        var tag = _context.Tag.Find(id);

        if (tag == null)
            return NotFound();

        tag.Name = request.Name;
        tag.Color = request.Color;

        _context.SaveChanges();

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Delete(Guid id)
    {
        var tag = _context.Tag.Find(id);

        if (tag == null)
            return NotFound();

        return View(tag);
    }

    [HttpPost, ActionName("Delete")]
    public IActionResult DeleteConfirmed(Guid id)
    {
        var tag = _context.Tag.Find(id);

        if (tag == null)
            return NotFound();

        _context.Tag.Remove(tag);
        _context.SaveChanges();

        return RedirectToAction(nameof(Index));
    }
}