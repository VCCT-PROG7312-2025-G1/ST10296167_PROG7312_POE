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
        // Display relevant events and announcements by checking search filters and returning a view model
        public async Task<IActionResult> EventsAndAnnouncements(string? category, DateTime? startDate, DateTime? endDate, string? search)
        {
            // If employee, redirect to dashboard
            if (_signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Dashboard", "User");
            }

            // Check if user has searched
            bool userSearched = !string.IsNullOrEmpty(search);
            bool noSearchFilters = string.IsNullOrEmpty(category) && !startDate.HasValue && !endDate.HasValue;
            bool noSearch = !userSearched || noSearchFilters;

            // Populate view model
            if (noSearch)
            {
                Console.WriteLine("NO SEARCH HIT");
                var viewModel = new EventsAnnouncementsViewModel
                {
                    SelectedCategory = category,
                    StartDate = startDate,
                    EndDate = endDate,
                    Events = await _eventService.GetAllEventsAsync(),
                    Announcements = await _announcementService.GetRecentAnnouncementsAsync(10),
                    Categories = await _eventService.GetAllCategoriesAsync(),
                    RecommendedEvents = await _eventService.GetCurrentRecommendationsAsync()
                };
                return View(viewModel);
            }
            else
            {
                Console.WriteLine("SEARCH HIT");
                var viewModel = new EventsAnnouncementsViewModel
                {
                    SelectedCategory = category,
                    StartDate = startDate,
                    EndDate = endDate,
                    Events = await _eventService.SearchEventsAsync(category, startDate, endDate),
                    Announcements = await _announcementService.GetRecentAnnouncementsAsync(10),
                    Categories = await _eventService.GetAllCategoriesAsync(),
                    RecommendedEvents = await _eventService.GetRecommendedEventsAsync()
                };
                return View(viewModel);
            }
        }
        
        // Display add event view
        public IActionResult AddEvent()
        {
            // If not employee, redirect to home
            if (!_signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        // Display add announcement view
        public IActionResult AddAnnouncement()
        {
            // If not employee, redirect to home
            if (!_signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        // Display all announcements view
        public async Task<IActionResult> AllAnnouncements()
        {
            // If employee, redirect to dashboard
            if (_signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Dashboard", "User");
            }

            // Populate view model
            var viewModel = new EventsAnnouncementsViewModel
            {
                Announcements = await _announcementService.GetAllAnnouncementsAsync(),
                Categories = await _eventService.GetAllCategoriesAsync()
            };
            return View(viewModel);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------//

        // Methods
        //------------------------------------------------------------------------------------------------------------------------------------------//
        // Add event to database and in-memory data structure
        [HttpPost]
        public async Task<IActionResult> AddEvent(Event newEvent)
        {
            // Check for input errors
            if (!ModelState.IsValid)
            {
                Console.WriteLine("Modelstate not valid");
                return View("AddEvent", newEvent);
            }

            // Add event
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

        // Add announcement to database and in-memory data structure
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
//--------------------------------------------------------X END OF FILE X-------------------------------------------------------------------//