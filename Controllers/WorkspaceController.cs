using Microsoft.AspNetCore.Mvc;
using TaskFlowMvc.Data;
using TaskFlowMvc.Models;
using TaskFlowMvc.Models.DTOs;

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
        try
        {
            var workspaces = _context.Workspace.ToList();
            return View(workspaces);
        }
        catch
        {
            TempData["Error"] = "Unable to load workspaces.";
            return View(new List<Workspace>());
        }
    }


    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(CreateWorkspaceRequest request)
    {
        if (!ModelState.IsValid)
            return View(request);

        try
        {
            var workspace = new Workspace
            {
                Name = request.Name,
                Description = request.Description,
            };

            _context.Workspace.Add(workspace);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
        catch
        {
            ModelState.AddModelError("", "Failed to create workspace.");
            return View(request);
        }
    }


    [HttpGet]
    public IActionResult Edit(Guid id)
    {
        try
        {
            var workspace = _context.Workspace.Find(id);

            if (workspace == null)
                return NotFound();

            var request = new UpdateWorkspaceRequest
            {
                Name = workspace.Name,
                Description = workspace.Description
            };

            return View(request);
        }
        catch
        {
            TempData["Error"] = "Unable to load workspace.";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    public IActionResult Edit(Guid id, UpdateWorkspaceRequest request)
    {
        if (!ModelState.IsValid)
            return View(request);

        try
        {
            var current = _context.Workspace.Find(id);

            if (current == null)
                return NotFound();

            current.Name = request.Name;
            current.Description = request.Description;
            Console.WriteLine(_context.GetType().FullName);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
        catch
        {
            ModelState.AddModelError("", "Failed to update workspace.");
            return View(request);
        }
    }


    [HttpGet]
    public IActionResult Delete(Guid id)
    {
        try
        {
            var workspace = _context.Workspace.Find(id);

            if (workspace == null)
                return NotFound();

            return View(workspace);
        }
        catch
        {
            TempData["Error"] = "Unable to load workspace.";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    public IActionResult DeleteConfirmed(Guid id)
    {
        try
        {
            var workspace = _context.Workspace.Find(id);

            if (workspace == null)
                return NotFound();

            workspace.IsDeleted = true;
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
        catch
        {
            TempData["Error"] = "Failed to delete workspace.";
            return RedirectToAction(nameof(Index));
        }
    }
}