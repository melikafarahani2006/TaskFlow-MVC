using Microsoft.AspNetCore.Mvc;
using TaskFlowMvc.Data;
using TaskFlowMvc.Models.Workspace;

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
        var workspaces = _context.Workspaces.ToList();

        return View(workspaces);
    }
    public IActionResult Create()
    {
        return View();
    }


    [HttpPost]
    public IActionResult Create(Workspace workspace)
    {
        workspace.CreatedAt = DateTime.UtcNow;

        _context.Workspaces.Add(workspace);
        _context.SaveChanges();

        return RedirectToAction(nameof(Index));
    }
}
