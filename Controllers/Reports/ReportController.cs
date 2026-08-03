using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stimulsoft.Report;
using Stimulsoft.Report.Mvc;
using TaskFlowMvc.Data;
using TaskFlowMvc.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using TaskFlowMvc.Models;

[Authorize]
public class ReportController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public ReportController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public IActionResult TaskReport(Guid? projectId)
    {
        if (projectId.HasValue && !GetAccessibleProjectIds().Contains(projectId.Value))
        {
            TempData["Error"] = "You don't have access to this project.";
            return RedirectToAction("Index", "Project");
        }

        ViewBag.ProjectId = projectId;
        return View();
    }

    private static string ToShamsi(DateTime? date)
    {
        if (!date.HasValue) return "-";

        var pc = new System.Globalization.PersianCalendar();
        var d = date.Value;

        return $"{pc.GetYear(d):0000}/{pc.GetMonth(d):00}/{pc.GetDayOfMonth(d):00}";
    }


    // Read data from Views (TaskReportView) in Db
    public IActionResult GetReport(Guid? projectId)
    {
        var accessibleWorkspaceIds = GetAccessibleWorkspaceIds();

        // Calling columns from TaskReportView directly👇
        //var query = _context.TaskReportView
        //    .Where(x => accessibleWorkspaceIds.Contains(x.WorkspaceId))
        //    .AsQueryable();

        //if (projectId.HasValue)
        //{
        //    query = query.Where(x => x.ProjectId == projectId.Value);
        //}

        //var tasks = query.ToList();


        // Using SP (Stored Procedure)👇
        var param = new Microsoft.Data.SqlClient.SqlParameter("@ProjectId", (object?)projectId ?? DBNull.Value);

        var tasks = _context.TaskReportView
            .FromSqlRaw("EXEC GetTaskReport @ProjectId", param)
            .AsEnumerable()
            .Where(x => accessibleWorkspaceIds.Contains(x.WorkspaceId))
            .ToList();


        foreach (var task in tasks)
        {
            task.CreatedAtShamsi = ToShamsi(task.CreatedAt);
            task.DueDateShamsi = ToShamsi(task.DueDate);
            task.UpdatedAtShamsi = ToShamsi(task.UpdatedAt);
        }

        var report = new StiReport();
        var path = Path.Combine(Directory.GetCurrentDirectory(), "Reports", "TaskReport.mrt");
        report.Load(path);

        report.RegData("Tasks", tasks);
        report.Dictionary.Synchronize();

        return StiNetCoreViewer.GetReportResult(this, report);
    }



    // Read data from application👇
    //public IActionResult GetReport(Guid? projectId)
    //{
    //    var accessibleWorkspaceIds = GetAccessibleWorkspaceIds();

    //    var query = _context.Task
    //        .Include(x => x.Project)
    //        .Include(x => x.TaskState)
    //        .Include(x => x.TaskTags)
    //        .ThenInclude(x => x.Tag)
    //        .Where(x => accessibleWorkspaceIds.Contains(x.Project.WorkspaceId))
    //        .AsQueryable();

    //    if (projectId.HasValue)
    //    {
    //        query = query.Where(x => x.ProjectId == projectId.Value);
    //    }

    //    var tasks = query
    //        .Select(x => new TaskResponse
    //        {
    //            Id = x.Id,
    //            Title = x.Title,
    //            Description = x.Description,
    //            DueDate = x.DueDate,
    //            Order = x.Order,

    //            ProjectId = x.ProjectId,
    //            ProjectName = x.Project.Name,

    //            TaskStateId = x.TaskStateId,
    //            TaskStateName = x.TaskState.Name,

    //            CreatedAt = x.CreatedAt,
    //            UpdatedAt = x.UpdatedAt,

    //            Tags = x.TaskTags
    //                .Select(t => new Tag
    //                {
    //                    Id = t.Tag.Id,
    //                    Name = t.Tag.Name,
    //                    Color = t.Tag.Color
    //                })
    //                .ToList(),
    //            TagsText = string.Join(", ", x.TaskTags.Select(t => t.Tag.Name))
    //        })
    //        .ToList();

    //    var report = new StiReport();
    //    var path = Path.Combine(Directory.GetCurrentDirectory(), "Reports", "TaskReport.mrt");
    //    report.Load(path);

    //    report.RegData("Tasks", tasks);
    //    report.Dictionary.Synchronize();

    //    return StiNetCoreViewer.GetReportResult(this, report);
    //}

    public IActionResult ViewerEvent()
    {
        return StiNetCoreViewer.ViewerEventResult(this);
    }

    private List<Guid> GetAccessibleWorkspaceIds()
    {
        if (User.IsInRole("Admin"))
            return _context.Workspace.Select(w => w.Id).ToList();

        var userId = _userManager.GetUserId(User);
        return _context.WorkspaceMember
            .Where(m => m.UserId == userId)
            .Select(m => m.WorkspaceId)
            .ToList();
    }

    private List<Guid> GetAccessibleProjectIds()
    {
        var workspaceIds = GetAccessibleWorkspaceIds();
        return _context.Project
            .Where(p => workspaceIds.Contains(p.WorkspaceId))
            .Select(p => p.Id)
            .ToList();
    }
}