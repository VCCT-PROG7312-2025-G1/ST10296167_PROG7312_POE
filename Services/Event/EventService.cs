using ST10296167_PROG7312_POE.Data;
using ST10296167_PROG7312_POE.Repository.Event;
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

            foreach(var kvp in _dataStore.EventsByDate)
            {
                // Sort events on the same day by time
                var eventsByTime = kvp.Value.OrderBy(e => e.Time);
                allEvents.AddRange(eventsByTime);
            }
            return await Task.FromResult(allEvents);
        }
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

        public Task<List<EventModel>> GetRecommendedEventsAsync()
        {
            return Task.FromResult(new List<EventModel>());
        }

        public Task<List<string>> GetAllCategoriesAsync()
        {
            // Clear set
            _dataStore.UniqueCategories.Clear();

            // Populate set
            foreach (var kvp in _dataStore.EventsByDate)
            {
                foreach (var ev in kvp.Value)
                {
                    _dataStore.UniqueCategories.Add(ev.Category);
                }
            }

            var categories = _dataStore.UniqueCategories.ToList();
            return Task.FromResult(categories);
        }

        public Task<List<EventModel>> SearchEventsAsync(string? category, DateTime? startDate, DateTime? endDate)
        {
            var filteredEvents = new List<EventModel>();

            var from = startDate ?? DateTime.MinValue;
            var to = endDate ?? DateTime.MaxValue;

            foreach(var kvp in _dataStore.EventsByDate)
            {
                if(kvp.Key >= from && kvp.Key <= to)
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
                StartDate = startDate,
                EndDate = endDate,
                SearchTimestamp = DateTime.UtcNow
            });

            return Task.FromResult(filteredEvents);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------//
    }
}
//--------------------------------------------------------X END OF FILE X-------------------------------------------------------------------//