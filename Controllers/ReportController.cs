using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ST10296167_PROG7312_POE.Models;
using ST10296167_PROG7312_POE.Services.Report;

namespace ST10296167_PROG7312_POE.Controllers
{
    public class ReportController : Controller
    {
        private readonly IReportService _reportService;
        private readonly SignInManager<User> _signInManager;
        
        // Constructor
        //------------------------------------------------------------------------------------------------------------------------------------------//
        public ReportController(IReportService reportService, SignInManager<User> signInManager)
        {
            _reportService = reportService;
            _signInManager = signInManager;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------//

        // Views
        //------------------------------------------------------------------------------------------------------------------------------------------//
        public IActionResult Report()
        {
            // If user is logged in, redirect them to user dashboard
            if (_signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Dashboard", "User");
            }
            
            return View();
        }

        public IActionResult ReportMenu()
        {
            // If user is logged in, redirect them to user dashboard
            if (_signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Dashboard", "User");
            }
            
            return View();
        }

        public IActionResult ReportList()
        {
            // If user is logged in, redirect them to user dashboard
            if (_signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Dashboard", "User");
            }
            
            var issues = _reportService.GetAllIssues();
            return View(issues);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------//

        // Methods
        //------------------------------------------------------------------------------------------------------------------------------------------//
        // Check if the user had input valid data for an issue and then add the issue to the datastore dictionary and db
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

        // Save a user's submitted rating and feedback to the Feedback linked list and db
        [HttpPost]
        public async Task<IActionResult> SubmitRating(int rating, string? feedback)
        {
            if (rating < 1 || rating > 5)
            {
                TempData["RatingError"] = "Please select a valid rating.";
                TempData["ShowRatingModal"] = true;
                return RedirectToAction("Report");
            }

            try
            {   
                await _reportService.SaveFeedback(rating, feedback);

                TempData["RatingSuccess"] = "Feedback received";
                return RedirectToAction("ReportMenu", "Report");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving rating: {ex.Message}");
                TempData["RatingError"] = "Failed to save rating. Please try again.";
                TempData["ShowRatingModal"] = true;
                return RedirectToAction("Report");
            }
        }

        // View an image file in the browser
        public IActionResult ViewFile(int fileId)
        {
            var file = _reportService.GetFileById(fileId);

            if (file == null)
            {
                TempData["ErrorMessage"] = "File not found.";
                return RedirectToAction("ReportList");
            }

            if (!System.IO.File.Exists(file.FilePath))
            {
                TempData["ErrorMessage"] = "File no longer exists on disk.";
                return RedirectToAction("ReportList");
            }

            try
            {
                var fileBytes = System.IO.File.ReadAllBytes(file.FilePath);

                if (file.MimeType.StartsWith("image/"))
                {
                    Response.Headers.Add("Content-Disposition", "inline");
                }

                return File(fileBytes, file.MimeType);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error viewing file: {ex.Message}");
                TempData["ErrorMessage"] = "Error occurred while loading the file.";
                return RedirectToAction("ReportList");
            }
        }

        // Download a selected file to a user's local machine if it still exists on disk
        public IActionResult DownloadFile(int fileId)
        {
            var file = _reportService.GetFileById(fileId);

            if (file == null)
            {
                TempData["ErrorMessage"] = "File not found.";
                return RedirectToAction("ReportList");
            }

            if (!System.IO.File.Exists(file.FilePath))
            {
                TempData["ErrorMessage"] = "File no longer exists on disk.";
                return RedirectToAction("ReportList");
            }

            try
            {
                var fileBytes = System.IO.File.ReadAllBytes(file.FilePath);
                return File(fileBytes, file.MimeType, file.FileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error downloading file: {ex.Message}");
                TempData["ErrorMessage"] = "Error occurred while downloading the file.";
                return RedirectToAction("ReportList");
            }
        }
        //------------------------------------------------------------------------------------------------------------------------------------------//
    }
}
//--------------------------------------------------------X END OF FILE X-------------------------------------------------------------------//