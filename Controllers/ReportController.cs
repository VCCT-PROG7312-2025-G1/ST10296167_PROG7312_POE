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
                TempData["ShowRatingModal"] = true;
                TempData["SuccessMessage"] = "Your issue has been submitted successfully!";
                return RedirectToAction("Report", new Issue());
            }
            else
            {
                return View("Report", issue);
            }

        }

        [HttpPost]
        public IActionResult SubmitRating(int rating, string? feedback)
        {
            if (rating < 1 || rating > 5)
            {
                TempData["RatingError"] = "Please select a valid rating.";
                TempData["ShowRatingModal"] = true;
                return RedirectToAction("Report");
            }

            try
            {   
                _reportService.SaveFeedback(rating, feedback);

                TempData["RatingSuccess"] = "Thank you for your feedback!";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving rating: {ex.Message}");
                TempData["RatingError"] = "Failed to save rating. Please try again.";
                TempData["ShowRatingModal"] = true;
                return RedirectToAction("Report");
            }
        }
        //------------------------------------------------------------------------------------------------------------------------------------------//
    }
}
//--------------------------------------------------------X END OF FILE X-------------------------------------------------------------------//