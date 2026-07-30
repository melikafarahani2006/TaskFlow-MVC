using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stimulsoft.Report;
using Stimulsoft.Report.Mvc;


using TaskFlowMvc.Data;
using TaskFlowMvc.Models;
using TaskFlowMvc.Models.DTOs;

namespace TaskFlowMvc.Controllers;

public class ReportController : Controller
{
    private readonly ApplicationDbContext _context;

    public ReportController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult TaskReport()
    {
        return View();
    }

    public IActionResult GetReport()
    {
        var tasks = _context.Task
            .Include(x => x.Project)
            .Include(x => x.TaskState)
            .Include(x => x.TaskTags)
            .ThenInclude(x => x.Tag)
            .Select(x => new TaskResponse
            {
                Id = x.Id,
                Title = x.Title,
                Description = x.Description,
                DueDate = x.DueDate,
                Order = x.Order,

                ProjectId = x.ProjectId,
                ProjectName = x.Project.Name,

                TaskStateId = x.TaskStateId,
                TaskStateName = x.TaskState.Name,

                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,

                Tags = x.TaskTags
                    .Select(t => new Tag
                    {
                        Id = t.Tag.Id,
                        Name = t.Tag.Name,
                        Color = t.Tag.Color
                    })
                    .ToList(),
                    TagsText = string.Join(", ", x.TaskTags.Select(t => t.Tag.Name))

            })
            .ToList();

        var report = new StiReport();
        var path = Path.Combine(
    Directory.GetCurrentDirectory(),
    "Reports",
    "TaskReport.mrt");

        report.Load(path);

        //report.RegBusinessObject("Tasks", tasks);
        //report.Dictionary.SynchronizeBusinessObjects();
        report.RegData("Tasks", tasks);
        report.Dictionary.Synchronize();

        return StiNetCoreViewer.GetReportResult(this, report);
    }

    public IActionResult ViewerEvent()
    {
        return StiNetCoreViewer.ViewerEventResult(this);
    }
}