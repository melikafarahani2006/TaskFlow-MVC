using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TaskFlowMvc.Data;
using TaskFlowMvc.Models;
using TaskFlowMvc.Models.DTOs;

namespace TaskFlowMvc.Controllers;

public class TaskItemController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<TaskItemController> _logger;

    public TaskItemController(ApplicationDbContext context, ILogger<TaskItemController> logger)
    {
        _context = context;
        _logger = logger;
    }


    public IActionResult Index()
    {
        try
        {
            var tasks = _context.TaskItem
                .Include(x => x.Project)
                .Include(x => x.TaskState)
                 .Include(x => x.TaskItemTags)
                .ThenInclude(x => x.Tag)
                .ToList();

            return View(tasks);
        }
        catch
        {
            TempData["Error"] = "Unable to load tasks.";
            return View(new List<TaskItem>());
        }
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
    public IActionResult Create(CreateTaskItemRequest request)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Project = new SelectList(_context.Project, "Id", "Name");
            ViewBag.TaskState = new SelectList(_context.TaskState, "Id", "Name");
            ViewBag.Tag = _context.Tag.ToList();

            return View(request);
        }

        try
        {
            var maxOrder = _context.TaskItem
                 .Where(x => x.ProjectId == request.ProjectId)
                 .Select(x => (int?)x.Order)
                 .Max() ?? 0;

            var task = new TaskItem
            {
                ProjectId = request.ProjectId,
                TaskStateId = request.TaskStateId,
                Title = request.Title,
                Description = request.Description,
                Order = maxOrder + 1,
                CreatedAt = DateTime.UtcNow
            };

            _context.TaskItem.Add(task);
            _context.SaveChanges();

            foreach (var tagId in request.TagIds)
            {
                _context.TaskItemTag.Add(new TaskItemTag
                {
                    TaskItemId = task.Id,
                    TagId = tagId
                });
            }

            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
        catch(Exception ex)
        {
            _logger.LogError(ex,
                   "Error while creating task.");

            ModelState.AddModelError("",
                "Failed to create task.");

            ViewBag.Project = new SelectList(_context.Project, "Id", "Name");
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
            var task = _context.TaskItem.Find(id);

            if (task == null)
                return NotFound();

            ViewBag.Project = new SelectList(_context.Project, "Id", "Name", task.ProjectId);
            ViewBag.TaskState = new SelectList(_context.TaskState, "Id", "Name", task.TaskStateId);
            ViewBag.Tag = _context.Tag.ToList();

            var selectedTags = _context.TaskItemTag
                .Where(x => x.TaskItemId == id)
                .Select(x => x.TagId)
                .ToList();

            return View(new UpdateTaskItemRequest
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
    public IActionResult Edit(Guid id, UpdateTaskItemRequest request)
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
            var task = _context.TaskItem.Find(id);

            if (task == null)
                return NotFound();

            task.ProjectId = request.ProjectId;
            task.TaskStateId = request.TaskStateId;
            task.Title = request.Title;
            task.Description = request.Description;

            var oldTags = _context.TaskItemTag
                .Where(x => x.TaskItemId == id);

            _context.TaskItemTag.RemoveRange(oldTags);

            foreach (var tagId in request.TagIds)
            {
                _context.TaskItemTag.Add(new TaskItemTag
                {
                    TaskItemId = id,
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
            var task = _context.TaskItem
                .Include(x => x.Project)
                .Include(x => x.TaskState)
                .Include(x => x.TaskItemTags)
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
            var task = _context.TaskItem.Find(id);

            if (task == null)
                return NotFound();

            _context.TaskItem.Remove(task);
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