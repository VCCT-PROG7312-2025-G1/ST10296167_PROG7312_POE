using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ST10296167_PROG7312_POE.Models;

namespace ST10296167_PROG7312_POE.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SignInManager<User> _signInManager;

        // Controller
        //------------------------------------------------------------------------------------------------------------------------------------------//
        public HomeController(ILogger<HomeController> logger, SignInManager<User> signInManager)
        {
            _logger = logger;
            _signInManager = signInManager;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------//

        // Views
        //------------------------------------------------------------------------------------------------------------------------------------------//
        public IActionResult Index()
        {
            // If user is logged in, redirect them to user dashboard
            if (_signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Dashboard", "User");
            }
            
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        //------------------------------------------------------------------------------------------------------------------------------------------//
    }
}
//--------------------------------------------------------X END OF FILE X-------------------------------------------------------------------//