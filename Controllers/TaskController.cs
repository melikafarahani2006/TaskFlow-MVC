using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TaskFlowMvc.Data;
using TaskFlowMvc.Models;
using TaskFlowMvc.Models.DTOs;

namespace TaskFlowMvc.Controllers;

[Authorize]
public class TaskController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<TaskController> _logger;
    private readonly UserManager<IdentityUser> _userManager;

    public TaskController(ApplicationDbContext context, ILogger<TaskController> logger, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _logger = logger;
        _userManager = userManager;
    }


    public IActionResult Index(Guid? projectId)
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

            var query = _context.Task
                .Include(x => x.Project)
                .Include(x => x.TaskState)
                .Include(x => x.TaskTags)
                .ThenInclude(x => x.Tag)
                .Where(x => memberWorkspaceIds.Contains(x.Project.WorkspaceId))
                .AsQueryable();

            if (projectId.HasValue)
            {
                if (!isAdmin && !_context.Project.Any(p => p.Id == projectId.Value && memberWorkspaceIds.Contains(p.WorkspaceId)))
                {
                    TempData["Error"] = "You don't have access to this project.";
                    return RedirectToAction("Index", "Project");
                }

                query = query.Where(x => x.ProjectId == projectId.Value);
            }

            var tasks = query.ToList();

            ViewBag.ProjectId = projectId;

            return View(tasks);
        }
        catch
        {
            TempData["Error"] = "Unable to load tasks.";
            return View(new List<Models.Task>());
        }
    }

    private List<Guid> GetAccessibleProjectIds()
    {
        if (User.IsInRole("Admin"))
        {
            return _context.Project.Select(p => p.Id).ToList();
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
        ViewBag.Project = new SelectList(_context.Project, "Id", "Name");
        ViewBag.TaskState = new SelectList(_context.TaskState, "Id", "Name");
        ViewBag.Tag = _context.Tag.ToList();

        return View();
    }

    [HttpPost]
    public IActionResult Create(CreateTaskRequest request)
    {
        var accessibleProjectIds = GetAccessibleProjectIds();

        if (!ModelState.IsValid)
        {
            ViewBag.Project = new SelectList(_context.Project.Where(p => accessibleProjectIds.Contains(p.Id)), "Id", "Name");
            ViewBag.TaskState = new SelectList(_context.TaskState, "Id", "Name");
            ViewBag.Tag = _context.Tag.ToList();

            return View(request);
        }

        if (!accessibleProjectIds.Contains(request.ProjectId))
        {
            TempData["Error"] = "You don't have access to this project.";
            return RedirectToAction("Index", "Project");
        }

        try
        {
            var maxOrder = _context.Task
                 .Where(x => x.ProjectId == request.ProjectId)
                 .Select(x => (int?)x.Order)
                 .Max() ?? 0;

            var task = new Models.Task
            {
                ProjectId = request.ProjectId,
                TaskStateId = request.TaskStateId,
                Title = request.Title,
                Description = request.Description,
                Order = maxOrder + 1,
            };

            _context.Task.Add(task);
            _context.SaveChanges();

            foreach (var tagId in request.TagIds)
            {
                _context.TaskTag.Add(new TaskTag
                {
                    TaskId = task.Id,
                    TagId = tagId
                });
            }

            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while creating task.");

            ModelState.AddModelError("", "Failed to create task.");

            ViewBag.Project = new SelectList(_context.Project.Where(p => accessibleProjectIds.Contains(p.Id)), "Id", "Name");
            ViewBag.TaskState = new SelectList(_context.TaskState, "Id", "Name");
            ViewBag.Tag = _context.Tag.ToList();

            return View(request);
        }
    }


    [HttpGet]
    public IActionResult Edit(Guid id)
    {
        try
        {
            var task = _context.Task.Find(id);

            if (task == null)
                return NotFound();

            ViewBag.Project = new SelectList(_context.Project, "Id", "Name", task.ProjectId);
            ViewBag.TaskState = new SelectList(_context.TaskState, "Id", "Name", task.TaskStateId);
            ViewBag.Tag = _context.Tag.ToList();

            var selectedTags = _context.TaskTag
                .Where(x => x.TaskId == id)
                .Select(x => x.TagId)
                .ToList();

            return View(new UpdateTaskRequest
            {
                ProjectId = task.ProjectId,
                TaskStateId = task.TaskStateId,
                Title = task.Title,
                Description = task.Description,
                TagIds = selectedTags
            });
        }
        catch
        {
            TempData["Error"] = "Unable to load task.";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    public IActionResult Edit(Guid id, UpdateTaskRequest request)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Project = new SelectList(_context.Project, "Id", "Name", request.ProjectId);
            ViewBag.TaskState = new SelectList(_context.TaskState, "Id", "Name", request.TaskStateId);
            ViewBag.Tag = _context.Tag.ToList();

            return View(request);
        }

        try
        {
            var task = _context.Task.Find(id);

            if (task == null)
                return NotFound();

            task.ProjectId = request.ProjectId;
            task.TaskStateId = request.TaskStateId;
            task.Title = request.Title;
            task.Description = request.Description;

            var oldTags = _context.TaskTag
                .Where(x => x.TaskId == id);

            _context.TaskTag.RemoveRange(oldTags);

            foreach (var tagId in request.TagIds)
            {
                _context.TaskTag.Add(new TaskTag
                {
                    TaskId = id,
                    TagId = tagId
                });
            }

            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
        catch
        {
            ModelState.AddModelError("", "Failed to update task.");

            ViewBag.Project = new SelectList(_context.Project, "Id", "Name", request.ProjectId);
            ViewBag.TaskState = new SelectList(_context.TaskState, "Id", "Name", request.TaskStateId);
            ViewBag.Tag = _context.Tag.ToList();

            return View(request);
        }
    }


    [HttpGet]
    public IActionResult Delete(Guid id)
    {
        try
        {
            var task = _context.Task
                .Include(x => x.Project)
                .Include(x => x.TaskState)
                .Include(x => x.TaskTags)
                .ThenInclude(x => x.Tag).ToList()
                .FirstOrDefault(x => x.Id == id);

            if (task == null)
                return NotFound();

            return View(task);
        }
        catch
        {
            TempData["Error"] = "Unable to load task.";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    public IActionResult DeleteConfirmed(Guid id)
    {
        try
        {
            var task = _context.Task.Find(id);

            if (task == null)
                return NotFound();

            task.IsDeleted = true;
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
        catch
        {
            TempData["Error"] = "Failed to delete task.";
            return RedirectToAction(nameof(Index));
        }
    }
}