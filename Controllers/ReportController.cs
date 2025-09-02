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
        public async Task<IActionResult> SubmitIssueReport(Issue issue, IFormFile[]? files)
        {
            if (!ModelState.IsValid)
            {
                Console.WriteLine("Modelstate not valid");
                return View("Report", issue);
            }

            var result = await _reportService.AddIssueAsync(issue, files);

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
//--------------------------------------------------------X END OF FILE X-------------------------------------------------------------------//