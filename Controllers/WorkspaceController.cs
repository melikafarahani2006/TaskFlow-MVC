using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskFlowMvc.Data;
using TaskFlowMvc.Models;
using TaskFlowMvc.Models.DTOs;
using TaskFlowMvc.Services;

namespace TaskFlowMvc.Controllers;

[Authorize]
public class WorkspaceController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly WorkspaceService _workspaceService;
    private readonly UserManager<IdentityUser> _userManager;


    public WorkspaceController(ApplicationDbContext context, WorkspaceService workspaceService, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _workspaceService = workspaceService;
        _userManager = userManager;

    }


    public IActionResult Index()
    {
        try
        {
            List<Workspace> workspaces;

            if (User.IsInRole("Admin"))
            {
                workspaces = _context.Workspace.ToList();
            }
            else
            {
                var userId = _userManager.GetUserId(User);

                workspaces = _context.WorkspaceMember
                    .Where(m => m.UserId == userId)
                    .Select(m => m.Workspace)
                    .ToList();
            }

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

    //[HttpPost]
    //public async Task<IActionResult> Create(CreateWorkspaceRequest request)
    //{
    //    if (!ModelState.IsValid)
    //        return View(request);

    //    try
    //    {

    //        await _workspaceService.CreateWorkspaceWithDefaultProject(request);

    //        TempData["Success"] = "Workspace created successfully.";

    //        return RedirectToAction(nameof(Index));
    //    }
    //    catch(Exception ex)
    //    {
    //        ModelState.AddModelError("", "Failed to create workspace.");
    //        return View(request);
    //    }
    //}


    [HttpPost]
    public IActionResult Create(CreateWorkspaceRequest request)
    {
        if (!ModelState.IsValid)
            return View(request);

        try
        {
            var userId = _userManager.GetUserId(User);

            var workspace = new Workspace
            {
                Name = request.Name,
                Description = request.Description,
            };

            _context.Workspace.Add(workspace);
            _context.SaveChanges();

            _context.WorkspaceMember.Add(new WorkspaceMember
            {
                WorkspaceId = workspace.Id,
                UserId = userId!,
                Role = "Owner"
            });

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


            var hasProjects = _context.Project
                .Any(x => x.WorkspaceId == id);

            if (hasProjects)
            {
                TempData["Error"] =
                    "Workspace cannot be deleted. Move the projects to another workspace or delete them first.";

                return RedirectToAction(nameof(Index));
            }

            workspace.IsDeleted = true;

            _context.SaveChanges();

            TempData["Success"] = "Workspace deleted successfully.";

            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException)
        {
            TempData["Error"] = "Failed to delete workspace.";

            return RedirectToAction(nameof(Index));
        }
    }
}