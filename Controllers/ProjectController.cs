using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TaskFlowMvc.Data;
using TaskFlowMvc.Models;
using TaskFlowMvc.Models.DTOs;

namespace TaskFlowMvc.Controllers;
public class ProjectController : Controller
{
    private readonly ApplicationDbContext _context;

    public ProjectController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var projects = _context.Project
                       .Include(x => x.Workspace)
                       .ToList();

        return View(projects);
    }

    [HttpGet]
    public IActionResult Create()
    {
        ViewBag.Workspace = new SelectList(
            _context.Workspace,
            "Id",
            "Name");

        return View();
    }

    [HttpPost]
    public IActionResult Create(Guid workspaceId, CreateProjectRequest request)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Workspace = new SelectList(
                _context.Workspace,
                "Id",
                "Name");

            return View(request);
        }

        var project = new Project
        {
            WorkspaceId = workspaceId,
            Name = request.Name,
            Description = request.Description,
            CreatedAt = DateTime.UtcNow
        };

        _context.Project.Add(project);

        _context.SaveChanges();

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Edit(Guid id)
    {
        var project = _context.Project.Find(id);

        if (project == null)
            return NotFound();

        ViewBag.Workspace = new SelectList(
            _context.Workspace,
            "Id",
            "Name",
            project.WorkspaceId);

        var request = new UpdateProjectRequest
        {
            Name = project.Name,
            Description = project.Description
        };

        return View(request);
    }

    [HttpPost]
    public IActionResult Edit(Guid id, UpdateProjectRequest request)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Workspace = new SelectList(
                _context.Workspace,
                "Id",
                "Name");

            return View(request);
        }

        var project = _context.Project.Find(id);

        if (project == null)
            return NotFound();

        project.Name = request.Name;
        project.Description = request.Description;

        _context.SaveChanges();

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Delete(Guid id)
    {
        var project = _context.Project
            .Include(x => x.Workspace)
            .FirstOrDefault(x => x.Id == id);

        if (project == null)
            return NotFound();

        return View(project);
    }

    [HttpPost]
    public IActionResult DeleteConfirmed(Guid id)
    {
        var project = _context.Project.Find(id);

        if (project == null)
            return NotFound();

        _context.Project.Remove(project);

        _context.SaveChanges();

        return RedirectToAction(nameof(Index));
    }
}
