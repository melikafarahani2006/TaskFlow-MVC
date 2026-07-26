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
        var taskStates = _context.TaskState.ToList();

        return View(taskStates);
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

        var taskState = new TaskState
        {
            Name = request.Name
        };

        _context.TaskState.Add(taskState);
        _context.SaveChanges();

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Edit(Guid id)
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

    [HttpPost]
    public IActionResult Edit(Guid id, UpdateTaskStateRequest request)
    {
        if (!ModelState.IsValid)
            return View(request);

        var taskState = _context.TaskState.Find(id);

        if (taskState == null)
            return NotFound();

        taskState.Name = request.Name;

        _context.SaveChanges();

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Delete(Guid id)
    {
        var taskState = _context.TaskState.Find(id);

        if (taskState == null)
            return NotFound();

        return View(taskState);
    }

    [HttpPost, ActionName("Delete")]
    public IActionResult DeleteConfirmed(Guid id)
    {
        var taskState = _context.TaskState.Find(id);

        if (taskState == null)
            return NotFound();

        _context.TaskState.Remove(taskState);
        _context.SaveChanges();

        return RedirectToAction(nameof(Index));
    }
}