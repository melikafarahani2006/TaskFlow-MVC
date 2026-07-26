using Microsoft.AspNetCore.Mvc;
using TaskFlowMvc.Data;
using TaskFlowMvc.Models.DTOs;
using TaskFlowMvc.Models;

namespace TaskFlowMvc.Controllers;
public class WorkspaceController : Controller
{
    private readonly ApplicationDbContext _context;

    public WorkspaceController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var workspaces = _context.Workspace.ToList();

        return View(workspaces);
    }
    public IActionResult Create()
    {
        return View();
    }


    [HttpPost]
    public IActionResult Create(CreateWorkspaceRequest request)
    {
        if (!ModelState.IsValid)
        {
            return View(request);
        }

        var workspace = new Workspace
        {
            Name = request.Name,
            Description = request.Description,
            CreatedAt = DateTime.UtcNow
        };

        _context.Workspace.Add(workspace);
        _context.SaveChanges();

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Edit(Guid id)
    {
        var workspace = _context.Workspace.Find(id);

        if (workspace == null)
            return NotFound();

        return View(workspace);
    }

    [HttpPost]
    public IActionResult Edit(Guid Id, UpdateWorkspaceRequest request)
    {
        var current = _context.Workspace.Find(request.Id);

        if (current == null)
            return NotFound();

        current.Name = request.Name;
        current.Description = request.Description;

        _context.SaveChanges();

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Delete(Guid id)
    {
        var workspace = _context.Workspace.Find(id);
        if (workspace == null)
            return NotFound();

        return View(workspace);
    }

    [HttpPost]
    public IActionResult DeleteConfirmed(Guid id)
    {
        var workspace = _context.Workspace.Find(id);
        if (workspace == null)
            return NotFound();

        _context.Workspace.Remove(workspace);
        _context.SaveChanges();

        return RedirectToAction(nameof(Index));
    }
}
