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
        try
        {
            return View(_context.Tag.ToList());
        }
        catch
        {
            TempData["Error"] = "Unable to load tags.";
            return View(new List<Tag>());
        }
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

        try
        {
            var tag = new Tag
            {
                Name = request.Name,
                Color = request.Color
            };

            _context.Tag.Add(tag);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
        catch
        {
            ModelState.AddModelError("", "Failed to create tag.");
            return View(request);
        }
    }


    [HttpGet]
    public IActionResult Edit(Guid id)
    {
        try
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
        catch
        {
            TempData["Error"] = "Unable to load tag.";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    public IActionResult Edit(Guid id, UpdateTagRequest request)
    {
        if (!ModelState.IsValid)
            return View(request);

        try
        {
            var tag = _context.Tag.Find(id);

            if (tag == null)
                return NotFound();

            tag.Name = request.Name;
            tag.Color = request.Color;

            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
        catch
        {
            ModelState.AddModelError("", "Failed to update tag.");
            return View(request);
        }
    }


    [HttpGet]
    public IActionResult Delete(Guid id)
    {
        try
        {
            var tag = _context.Tag.Find(id);

            if (tag == null)
                return NotFound();

            return View(tag);
        }
        catch
        {
            TempData["Error"] = "Unable to load tag.";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    public IActionResult DeleteConfirmed(Guid id)
    {
        try
        {
            var tag = _context.Tag.Find(id);

            if (tag == null)
                return NotFound();

            var isUsed = _context.TaskTag.Any(x => x.TagId == id);

            if (isUsed)
            {
                TempData["Error"] =
                    "Tag cannot be deleted. Remove it from all tasks first.";

                return RedirectToAction(nameof(Index));
            }

            tag.IsDeleted = true;

            _context.SaveChanges();

            TempData["Success"] = "Tag deleted successfully.";

            return RedirectToAction(nameof(Index));
        }
        catch
        {
            TempData["Error"] = "Failed to delete tag.";
            return RedirectToAction(nameof(Index));
        }
    }
}