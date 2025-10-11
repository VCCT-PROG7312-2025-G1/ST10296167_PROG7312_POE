using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ST10296167_PROG7312_POE.Models;
using ST10296167_PROG7312_POE.Services.Announcement;
using ST10296167_PROG7312_POE.Services.Event;

namespace ST10296167_PROG7312_POE.Controllers
{
    public class EventsAndAnnouncementsController: Controller
    {
        private readonly IEventService _eventService;
        private readonly IAnnouncementService _announcementService;
        private readonly SignInManager<User> _signInManager;

        // Constructor
        //------------------------------------------------------------------------------------------------------------------------------------------//
        public EventsAndAnnouncementsController(IEventService eventService, IAnnouncementService announcementService, SignInManager<User> signInManager)
        {
            _eventService = eventService;
            _announcementService = announcementService;
            _signInManager = signInManager;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------//

        // Views
        //------------------------------------------------------------------------------------------------------------------------------------------//
        public IActionResult EventsAndAnnouncements()
        {
            // If employee, redirect to dashboard
            if (_signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Dashboard", "User");
            }

            return View();
        }

        public IActionResult AddEvent()
        {
            // If not employee, redirect to home
            if (!_signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        public IActionResult AddAnnouncement()
        {
            // If not employee, redirect to home
            if (!_signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }
        //------------------------------------------------------------------------------------------------------------------------------------------//

        // Methods
        //------------------------------------------------------------------------------------------------------------------------------------------//
        [HttpPost]
        public async Task<IActionResult> AddEvent(Event newEvent)
        {
            if (!ModelState.IsValid)
            {
                Console.WriteLine("Modelstate not valid");
                return View("AddEvent", newEvent);
            }

            var result = await _eventService.AddEventAsync(newEvent);

            if (result)
            {
                return RedirectToAction("Dashboard", "User");
            }
            else
            {
                TempData["ErrorMessage"] = "An error occurred while adding the event. Please try again.";
                return View("AddEvent", newEvent);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddAnnouncement(Announcement announcement)
        {
            if (!ModelState.IsValid)
            {
                Console.WriteLine("Modelstate not valid");
                return View("AddAnnouncement", announcement);
            }
            var result = await _announcementService.AddAnnouncementAsync(announcement);
            if (result)
            {
                return RedirectToAction("Dashboard", "User");
            }
            else
            {
                TempData["ErrorMessage"] = "An error occurred while adding the announcement. Please try again.";
                return View("AddAnnouncement", announcement);
            }
        }
        //------------------------------------------------------------------------------------------------------------------------------------------//
    }
}
