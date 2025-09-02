using Microsoft.AspNetCore.Mvc;
using ST10296167_PROG7312_POE.Models;
using ST10296167_PROG7312_POE.Services.Report;

namespace ST10296167_PROG7312_POE.Controllers
{
    public class ReportController : Controller
    {
        private readonly IReportService _reportService;
        // Constructor
        //------------------------------------------------------------------------------------------------------------------------------------------//
        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------//

        // Views
        //------------------------------------------------------------------------------------------------------------------------------------------//
        public IActionResult Report()
        {
            return View();
        }
        //------------------------------------------------------------------------------------------------------------------------------------------//

        // Methods
        //------------------------------------------------------------------------------------------------------------------------------------------//
        [HttpPost]
        public IActionResult SubmitIssueReport(Issue issue)
        {
            if (!ModelState.IsValid)
            {
                Console.WriteLine("Modelstate not valud");
                return View("Report", issue);
            }

            var result = _reportService.AddIssueAsync(issue);
            Console.WriteLine(result);

            if (result)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View("Report", issue);
            }

        }
        //------------------------------------------------------------------------------------------------------------------------------------------//
    }
}
