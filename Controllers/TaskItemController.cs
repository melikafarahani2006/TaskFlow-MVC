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

    public TaskItemController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var tasks = _context.TaskItem
            .Include(x => x.Project)
            .Include(x => x.TaskState)
            .ToList();

        return View(tasks);
    }

    [HttpGet]
    public IActionResult Create()
    {
        ViewBag.Project = new SelectList(
            _context.Project,
            "Id",
            "Name");

        ViewBag.TaskState = new SelectList(
            _context.TaskState,
            "Id",
            "Name");

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

            return View(request);
        }

        var task = new TaskItem
        {
            ProjectId = request.ProjectId,
            TaskStateId = request.TaskStateId,
            Title = request.Title,
            Description = request.Description,
            Order = request.Order,
            IsCompleted = false,
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

    [HttpGet]
    public IActionResult Edit(Guid id)
    {
        var task = _context.TaskItem.Find(id);

        if (task == null)
            return NotFound();

        ViewBag.Project = new SelectList(
            _context.Project,
            "Id",
            "Name",
            task.ProjectId);

        ViewBag.TaskState = new SelectList(
            _context.TaskState,
            "Id",
            "Name",
            task.TaskStateId);

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
            IsCompleted = task.IsCompleted,
            TagIds = selectedTags,
            Order = task.Order
        });
    }

    [HttpPost]
    public IActionResult Edit(Guid id, UpdateTaskItemRequest request)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Project = new SelectList(_context.Project, "Id", "Name", request.ProjectId);
            ViewBag.TaskState = new SelectList(_context.TaskState, "Id", "Name", request.TaskStateId);

            return View(request);
        }

        var task = _context.TaskItem.Find(id);

        var oldTags = _context.TaskItemTag
            .Where(x => x.TaskItemId == id);

        _context.TaskItemTag.RemoveRange(oldTags);

        _context.SaveChanges();

        if (task == null)
            return NotFound();

        task.ProjectId = request.ProjectId;
        task.TaskStateId = request.TaskStateId;
        task.Title = request.Title;
        task.Description = request.Description;
        task.IsCompleted = request.IsCompleted;
        task.Order = request.Order;
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

    [HttpGet]
    public IActionResult Delete(Guid id)
    {
        var task = _context.TaskItem
            .Include(x => x.Project)
            .Include(x => x.TaskState)
            .FirstOrDefault(x => x.Id == id);

        if (task == null)
            return NotFound();

        return View(task);
    }

    [HttpPost, ActionName("Delete")]
    public IActionResult DeleteConfirmed(Guid id)
    {
        var task = _context.TaskItem.Find(id);

        if (task == null)
            return NotFound();

        _context.TaskItem.Remove(task);
        _context.SaveChanges();

        return RedirectToAction(nameof(Index));
    }
}