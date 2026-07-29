using Microsoft.AspNetCore.Mvc;

namespace TaskFlowMvc.Controllers;

public class ReportController : Controller
{
    public IActionResult TaskReport()
    {
        return View();
    }
}