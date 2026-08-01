using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Stimulsoft.Blockly.Model;
using TaskFlowMvc.Data;
using TaskFlowMvc.Models;
using TaskFlowMvc.Models.DTOs;

namespace TaskFlowMvc.Controllers;

[Authorize]
public class ProjectController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;


    public ProjectController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }


    public async Task<IActionResult> Index(Guid? workspaceId)
    {
        try
        {
            bool isAdmin = User.IsInRole("Admin");

            IQueryable<Guid> memberWorkspaceIds;

            if (isAdmin)
            {
                memberWorkspaceIds = _context.Workspace.Select(w => w.Id);
            }
            else
            {
                var userId = _userManager.GetUserId(User);
                memberWorkspaceIds = _context.WorkspaceMember
                    .Where(m => m.UserId == userId)
                    .Select(m => m.WorkspaceId);
            }

            IQueryable<Project> query = _context.Project
                .Include(p => p.Workspace)
                .Where(p => memberWorkspaceIds.Contains(p.WorkspaceId));

            if (workspaceId.HasValue)
            {
                if (!isAdmin && !memberWorkspaceIds.Contains(workspaceId.Value))
                {
                    TempData["Error"] = "You don't have access to this workspace.";
                    return RedirectToAction("Index", "Workspace");
                }

                query = query.Where(p => p.WorkspaceId == workspaceId.Value);
            }

            var projects = await query.ToListAsync();

            return View(projects);
        }
        catch
        {
            TempData["Error"] = "Unable to load projects.";
            return View(new List<Project>());
        }
    }

    private List<Guid> GetAccessibleWorkspaceIds()
    {
        if (User.IsInRole("Admin"))
        {
            return _context.Workspace.Select(w => w.Id).ToList();
        }

        var userId = _userManager.GetUserId(User);

        var memberWorkspaceIds = _context.WorkspaceMember
            .Where(m => m.UserId == userId)
            .Select(m => m.WorkspaceId);

        return _context.Project
            .Where(p => memberWorkspaceIds.Contains(p.WorkspaceId))
            .Select(p => p.Id)
            .ToList();
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
        var accessibleWorkspaceIds = GetAccessibleWorkspaceIds();

        if (!ModelState.IsValid)
        {
            ViewBag.Workspace = new SelectList(
                _context.Workspace,
                "Id",
                "Name");

            return View(request);
        }
        if (!accessibleWorkspaceIds.Contains(request.WorkspaceId))
        {
            TempData["Error"] = "You don't have access to this workspace.";
            return RedirectToAction("Index", "Workspace");
        }

        try
        {
            var project = new Project
            {
                WorkspaceId = workspaceId,
                Name = request.Name,
                Description = request.Description,
            };

            _context.Project.Add(project);

            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
        catch
        {
            ModelState.AddModelError("", "Failed to create project.");

            ViewBag.Workspace = new SelectList(
                _context.Workspace,
                "Id",
                "Name");

            return View(request);
        }
    }


    [HttpGet]
    public IActionResult Edit(Guid id)
    {
        try
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
        catch
        {
            TempData["Error"] = "Unable to load project.";
            return RedirectToAction(nameof(Index));
        }
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

        try
        {
            var project = _context.Project.Find(id);

            if (project == null)
                return NotFound();

            project.Name = request.Name;
            project.Description = request.Description;
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
        catch
        {
            ModelState.AddModelError("", "Failed to update project.");

            ViewBag.Workspace = new SelectList(
                _context.Workspace,
                "Id",
                "Name");

            return View(request);
        }
    }


    [HttpGet]
    public IActionResult Delete(Guid id)
    {
        try
        {
            var project = _context.Project
                .Include(x => x.Workspace)
                .FirstOrDefault(x => x.Id == id);

            if (project == null)
                return NotFound();

            return View(project);
        }
        catch
        {
            TempData["Error"] = "Unable to load project.";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    public IActionResult DeleteConfirmed(Guid id)
    {
        try
        {
            var project = _context.Project.Find(id);

            if (project == null)
                return NotFound();

            var hasTasks = _context.Task.Any(x => x.ProjectId == id);

            if (hasTasks)
            {
                TempData["Error"] =
                    "Project cannot be deleted. Move the tasks to another project or delete them first.";

                return RedirectToAction(nameof(Index));
            }

            project.IsDeleted = true;

            _context.SaveChanges();

            TempData["Success"] = "Project deleted successfully.";

            return RedirectToAction(nameof(Index));
        }
        catch
        {
            TempData["Error"] = "Failed to delete project.";
            return RedirectToAction(nameof(Index));
        }
    }
}