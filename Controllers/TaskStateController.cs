using Microsoft.AspNetCore.Mvc;
using TaskFlowMvc.Data;
using TaskFlowMvc.Models;
using TaskFlowMvc.Models.DTOs;

namespace TaskFlowMvc.Controllers;

public class TaskStateController : Controller
{
    private readonly ApplicationDbContext _context;

    public TaskStateController(ApplicationDbContext context)
    {
        _context = context;
    }


    public IActionResult Index()
    {
        try
        {
            var taskStates = _context.TaskState.ToList();
            return View(taskStates);
        }
        catch
        {
            TempData["Error"] = "Unable to load task states.";
            return View(new List<TaskState>());
        }
    }


    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(CreateTaskStateRequest request)
    {
        if (!ModelState.IsValid)
            return View(request);

        try
        {
            var taskState = new TaskState
            {
                Name = request.Name
            };

            _context.TaskState.Add(taskState);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
        catch
        {
            ModelState.AddModelError("", "Failed to create task state.");
            return View(request);
        }
    }


    [HttpGet]
    public IActionResult Edit(Guid id)
    {
        try
        {
            var taskState = _context.TaskState.Find(id);

            if (taskState == null)
                return NotFound();

            var request = new UpdateTaskStateRequest
            {
                Name = taskState.Name
            };

            return View(request);
        }
        catch
        {
            TempData["Error"] = "Unable to load task state.";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    public IActionResult Edit(Guid id, UpdateTaskStateRequest request)
    {
        if (!ModelState.IsValid)
            return View(request);

        try
        {
            var taskState = _context.TaskState.Find(id);

            if (taskState == null)
                return NotFound();

            taskState.Name = request.Name;

            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
        catch
        {
            ModelState.AddModelError("", "Failed to update task state.");
            return View(request);
        }
    }


    [HttpGet]
    public IActionResult Delete(Guid id)
    {
        try
        {
            var taskState = _context.TaskState.Find(id);

            if (taskState == null)
                return NotFound();

            return View(taskState);
        }
        catch
        {
            TempData["Error"] = "Unable to load task state.";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    public IActionResult DeleteConfirmed(Guid id)
    {
        try
        {
            var taskState = _context.TaskState.Find(id);

            if (taskState == null)
                return NotFound();

            var hasTasks = _context.Task.Any(x => x.TaskStateId == id);

            if (hasTasks)
            {
                TempData["Error"] =
                    "Task state cannot be deleted. Move the tasks to another state or delete them first.";

                return RedirectToAction(nameof(Index));
            }

            taskState.IsDeleted = true;

            _context.SaveChanges();

            TempData["Success"] = "Task state deleted successfully.";

            return RedirectToAction(nameof(Index));
        }
        catch
        {
            TempData["Error"] = "Failed to delete task state.";
            return RedirectToAction(nameof(Index));
        }
    }
}