using ST10296167_PROG7312_POE.Data;
using ST10296167_PROG7312_POE.Repository.Event;
using ST10296167_PROG7312_POE.Services.Recommendation;
using EventModel = ST10296167_PROG7312_POE.Models.Event;

namespace ST10296167_PROG7312_POE.Services.Event
{
    public class EventService: IEventService
    {
        private readonly DataStore _dataStore;
        private readonly IEventRepository _eventRepository;

        // Constructor
        //------------------------------------------------------------------------------------------------------------------------------------------//
        public EventService(IEventRepository eventRepository, DataStore dataStore)
        {
            _eventRepository = eventRepository;
            _dataStore = dataStore;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------//

        // Methods
        //------------------------------------------------------------------------------------------------------------------------------------------//
        public async Task<List<EventModel>> GetAllEventsAsync()
        {
            var allEvents = new List<EventModel>();

            // Only include current or future events
            var today = DateTime.Today;

            foreach (var kvp in _dataStore.EventsByDate)
            {
                if (kvp.Key >= today)
                {
                    // Sort events on the same day by time
                    var eventsByTime = kvp.Value.OrderBy(e => e.Time);
                    allEvents.AddRange(eventsByTime);
                }
            }
            
            return await Task.FromResult(allEvents);
        }

        // Add a new event to the database and dictionary
        public async Task<bool> AddEventAsync(EventModel newEvent)
        {
            try
            {
                // Add to DB
                var savedEvent = await _eventRepository.AddEventAsync(newEvent);
                newEvent.ID = savedEvent.ID;

                var eventDateTime = newEvent.Date.ToDateTime(newEvent.Time);

                if(!_dataStore.EventsByDate.ContainsKey(eventDateTime))
                {
                    _dataStore.EventsByDate[eventDateTime] = new List<EventModel>();
                }

                // Add to sorted dictionary
                _dataStore.EventsByDate[eventDateTime].Add(newEvent);
                Console.WriteLine($"Event added to in-memory datastore");

                return true;
            }
            catch
            {
                return false;
            }
        }

        // Get all event categories to populate the unique categories hash set
        public Task<List<string>> GetAllCategoriesAsync()
        {
            // Clear set
            _dataStore.UniqueCategories.Clear();

            foreach (var kvp in _dataStore.EventsByDate)
            {
                foreach (var ev in kvp.Value)
                {
                    _dataStore.UniqueCategories.Add(ev.Category);
                }
            }

            // Populate set
            var categories = _dataStore.UniqueCategories.ToList();

            return Task.FromResult(categories);
        }

        // Get current recommendations from the list
        public Task<List<EventModel>> GetCurrentRecommendationsAsync()
        {
            // Return cached recommendations list if available
            if (_dataStore.CurrentRecommendations != null && _dataStore.CurrentRecommendations.Any())
            {
                return Task.FromResult(_dataStore.CurrentRecommendations);
            }
            return Task.FromResult(new List<EventModel>());
        }

        // Find and return events based on search filters
        public Task<List<EventModel>> SearchEventsAsync(string? category, DateTime? startDate, DateTime? endDate)
        {
            var filteredEvents = new List<EventModel>();

            var from = startDate ?? DateTime.Today;
            var to = endDate ?? DateTime.MaxValue;

            foreach(var kvp in _dataStore.EventsByDate)
            {
                // Convert to date only
                var eventDate = DateOnly.FromDateTime(kvp.Key);   
                var fromDateOnly = DateOnly.FromDateTime(from);   
                var toDateOnly = DateOnly.FromDateTime(to);

                if (eventDate >= fromDateOnly && eventDate <= toDateOnly)
                {
                    filteredEvents.AddRange(kvp.Value);
                }
            }

            if (!string.IsNullOrEmpty(category))
            {
                filteredEvents = filteredEvents
                    .Where(e => e.Category == category)
                    .ToList();
            }

            _dataStore.LogSearch(new Models.SearchQuery
            {
                Category = category,
                StartDate = DateOnly.FromDateTime(from),
                EndDate = DateOnly.FromDateTime(to),
                SearchTimestamp = DateTime.UtcNow
            });

            return Task.FromResult(filteredEvents);
        }


        // Generate event recommendations based on search history
        public Task<List<EventModel>> GetRecommendedEventsAsync()
        {
            try
            {
                var searchHistory = _dataStore.SearchHistory;

                if (searchHistory == null || searchHistory.Count == 0)
                {
                    Console.WriteLine("No search history available for recommendations");
                    return Task.FromResult(new List<EventModel>());
                }

                // Get all future events
                var eventList = new List<EventModel>();
                var today = DateTime.Now;

                foreach (var kvp in _dataStore.EventsByDate)
                {
                    if (kvp.Key.Date >= today.Date)
                    {
                        eventList.AddRange(kvp.Value);
                    }
                }

                if (eventList.Count == 0)
                {
                    Console.WriteLine("No future events available for recommendations");
                    return Task.FromResult(new List<EventModel>());
                }

                // Generate recommendations
                var recommendations = new RecommendationsGenerator();
                var recommendedEvents = recommendations.GenerateRecommendations(searchHistory, eventList, maxRecommendations: 3);

                // Store recommendations in List
                _dataStore.CurrentRecommendations.Clear();
                _dataStore.CurrentRecommendations.AddRange(recommendedEvents);

                return Task.FromResult(recommendedEvents);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating recommendations: {ex.Message}");
                return Task.FromResult(new List<EventModel>());
            }
        }
        //------------------------------------------------------------------------------------------------------------------------------------------//
    }
}
//--------------------------------------------------------X END OF FILE X-------------------------------------------------------------------//