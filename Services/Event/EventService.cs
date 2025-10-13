using ST10296167_PROG7312_POE.Data;
using ST10296167_PROG7312_POE.Models;
using ST10296167_PROG7312_POE.Repository.Event;
using static System.Formats.Asn1.AsnWriter;
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

        public Task<List<EventModel>> GetCurrentRecommendationsAsync()
        {
            // Return cached recommendations if available
            if (_dataStore.CurrentRecommendations != null && _dataStore.CurrentRecommendations.Any())
            {
                Console.WriteLine("GOT FROM CURRENT RECS");
                return Task.FromResult(_dataStore.CurrentRecommendations);
            }
            return Task.FromResult(new List<EventModel>());
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

        public Task<List<EventModel>> GetRecommendedEventsAsync()
        {
            try
            {
                // Get search history
                var searchHistory = _dataStore.SearchHistory;

                if (searchHistory == null || searchHistory.Count == 0)
                {
                    Console.WriteLine("No search history available for recommendations");
                    return Task.FromResult(new List<EventModel>());
                }

                // Analyze search patterns to get user preferences
                var analysis = AnalyzeSearchPatterns(searchHistory);

                // Filter to only future events
                var allEvents = new List<EventModel>();
                var today = DateTime.Now;

                foreach (var kvp in _dataStore.EventsByDate)
                {
                    // Only include events that are today or in the future
                    if (kvp.Key.Date >= today.Date)
                    {
                        allEvents.AddRange(kvp.Value);
                    }
                }

                if (allEvents.Count == 0)
                {
                    Console.WriteLine("No future events available for recommendations");
                    return Task.FromResult(new List<EventModel>());
                }

                // Score each event based on user preferences
                var scoredEvents = new List<(EventModel Event, int Score)>();

                foreach (var ev in allEvents)
                {
                    int score = CalculateEventScore(ev, analysis);
                    scoredEvents.Add((ev, score));
                }

                // Sort by score (descending), then by date (ascending) for tiebreaker
                var recommendedEvents = scoredEvents
                    .Where(x => x.Score > 0) // Only include events with positive score
                    .OrderByDescending(x => x.Score)
                    .ThenBy(x => x.Event.Date)
                    .ThenBy(x => x.Event.Time)
                    .Select(x => x.Event)
                    .Take(3) // Return top 3
                    .ToList();

                Console.WriteLine($"Generated {recommendedEvents.Count} recommendations based on search history");

                // Save to list for refernce
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

        // Helper method: Analyze search patterns from history
        private SearchPatternAnalysis AnalyzeSearchPatterns(Queue<SearchQuery> searchHistory)
        {
            var analysis = new SearchPatternAnalysis();

            // Iterate through each search query
            foreach (var search in searchHistory)
            {

                // CATEGORY ANALYSIS

                if (!string.IsNullOrEmpty(search.Category))
                {
                    IncrementDictionary(analysis.CategoryFrequency, search.Category);
                    analysis.AllSearchedCategories.Add(search.Category);
                }

                // MONTH ANALYSIS

                // Analyze StartDate if provided
                if (search.StartDate.HasValue)
                {
                    int month = search.StartDate.Value.Month;
                    IncrementDictionary(analysis.MonthPreferences, month);
                }

                // Analyze EndDate if provided
                if (search.EndDate.HasValue)
                {
                    int month = search.EndDate.Value.Month;
                    IncrementDictionary(analysis.MonthPreferences, month);
                }

                // DATE RANGE ANALYSIS

                if (search.StartDate.HasValue && search.EndDate.HasValue)
                {
                    var rangeLength = (int)(search.EndDate.Value - search.StartDate.Value).TotalDays;

                    if (rangeLength <= 7)
                    {
                        // Short range (up to 1 week)
                        IncrementDictionary(analysis.DayRangePreferences, "near-term");
                    }
                    else if (rangeLength <= 30)
                    {
                        // Medium range (1-4 weeks)
                        IncrementDictionary(analysis.DayRangePreferences, "mid-term");
                    }
                    else
                    {
                        // Long range (1+ months)
                        IncrementDictionary(analysis.DayRangePreferences, "far-term");
                    }
                }
                else if (search.StartDate.HasValue || search.EndDate.HasValue)
                {
                    // Assume near-term if only one date is provided
                    IncrementDictionary(analysis.DayRangePreferences, "near-term");
                }
            }

            Console.WriteLine($"Search pattern analysis: {analysis.ToString()}");

            return analysis;
        }
        
        // Calculate score for a single event
        private int CalculateEventScore(EventModel ev, SearchPatternAnalysis analysis)
        {
            int score = 0;
            var today = DateTime.Now;
            var eventDateTime = ev.Date.ToDateTime(ev.Time);

            // CATEGORY SCORING (0-10 points)

            if (analysis.CategoryFrequency.Count > 0)
            {
                // Get sorted list of categories by frequency (highest first)
                var sortedCategories = analysis.CategoryFrequency
                    .OrderByDescending(x => x.Value)
                    .Select(x => x.Key)
                    .ToList();

                // Check if event category matches user's top categories
                if (sortedCategories.Count > 0 && ev.Category == sortedCategories[0])
                {
                    // most searched category +10 points
                    score += 10;
                    Console.WriteLine($"Event '{ev.Title}' matched top category '{ev.Category}': +10");
                }
                else if (sortedCategories.Count > 1 && ev.Category == sortedCategories[1])
                {
                    // 2ND most searched category +8 points
                    score += 8;
                    Console.WriteLine($"Event '{ev.Title}' matched 2nd category '{ev.Category}': +8");
                }
                else if (sortedCategories.Count > 2 && ev.Category == sortedCategories[2])
                {
                    // 3RD most searched category +6 points
                    score += 6;
                    Console.WriteLine($"Event '{ev.Title}' matched 3rd category '{ev.Category}': +6");
                }
                else if (analysis.AllSearchedCategories.Contains(ev.Category))
                {
                    // Category appears in search history but not top 3 +4 points
                    score += 4;
                    Console.WriteLine($"Event '{ev.Title}' matched searched category '{ev.Category}': +4");
                }
            }

            // MONTH PREFERENCE SCORING (0-4 points)

            int eventMonth = ev.Date.Month;

            if (analysis.MonthPreferences.Count > 0)
            {
                // Get the most frequently searched month
                var topMonth = analysis.MonthPreferences.OrderByDescending(x => x.Value).First().Key;

                if (eventMonth == topMonth)
                {
                    // Event is in user's most-searched month +4 points
                    score += 4;
                    Console.WriteLine($"Event '{ev.Title}' is in top month ({GetMonthName(eventMonth)}): +4");
                }
                else if (analysis.MonthPreferences.ContainsKey(eventMonth))
                {
                    // Event is in a month user has searched +2 points
                    score += 2;
                    Console.WriteLine($"Event '{ev.Title}' is in searched month ({GetMonthName(eventMonth)}): +2");
                }
            }

            // DAY RANGE / PROXIMITY SCORING (0-7 points)

            // Calculate days between today and event date
            int daysUntilEvent = (int)(eventDateTime.Date - today.Date).TotalDays;

            // Determine user's preferred search range
            string? userPreferredRange = analysis.GetPreferredRange();

            if (userPreferredRange == "near-term")
            {
                // User prefers near-term events (1 week)
                if (daysUntilEvent <= 3 && daysUntilEvent >= 0)
                {
                    score += 7;
                    Console.WriteLine($"Event '{ev.Title}' is within 3 days (user prefers near-term): +7");
                }
                else if (daysUntilEvent <= 7 && daysUntilEvent > 3)
                {
                    score += 5;
                    Console.WriteLine($"Event '{ev.Title}' is within 7 days (user prefers near-term): +5");
                }
                else if (daysUntilEvent <= 14 && daysUntilEvent > 7)
                {
                    score += 2;
                    Console.WriteLine($"Event '{ev.Title}' is within 14 days (less preferred): +2");
                }
                // Beyond 14 days = no bonus
            }
            else if (userPreferredRange == "mid-term")
            {
                // User prefers mid-term events (1-4 weeks)
                if (daysUntilEvent <= 7 && daysUntilEvent >= 0)
                {
                    score += 4;
                    Console.WriteLine($"Event '{ev.Title}' is within 7 days (user prefers mid-term): +4");
                }
                else if (daysUntilEvent <= 14 && daysUntilEvent > 7)
                {
                    score += 6;
                    Console.WriteLine($"Event '{ev.Title}' is within 14 days (user prefers mid-term): +6");
                }
                else if (daysUntilEvent <= 30 && daysUntilEvent > 14)
                {
                    score += 4;
                    Console.WriteLine($"Event '{ev.Title}' is within 30 days (user prefers mid-term): +4");
                }
                else if (daysUntilEvent <= 60 && daysUntilEvent > 30)
                {
                    score += 1;
                    Console.WriteLine($"Event '{ev.Title}' is within 60 days (less preferred): +1");
                }
            }
            else if (userPreferredRange == "far-term")
            {
                // User prefers far-term events (1+ months)
                if (daysUntilEvent <= 14 && daysUntilEvent >= 0)
                {
                    score += 2;
                    Console.WriteLine($"Event '{ev.Title}' is within 14 days (user prefers far-term): +2");
                }
                else if (daysUntilEvent <= 30 && daysUntilEvent > 14)
                {
                    score += 4;
                    Console.WriteLine($"Event '{ev.Title}' is within 30 days (user prefers far-term): +4");
                }
                else if (daysUntilEvent <= 60 && daysUntilEvent > 30)
                {
                    score += 6;
                    Console.WriteLine($"Event '{ev.Title}' is within 60 days (user prefers far-term): +6");
                }
                else if (daysUntilEvent > 60)
                {
                    score += 4;
                    Console.WriteLine($"Event '{ev.Title}' is 60+ days (user prefers far-term): +4");
                }
            }
            else
            {
                // No preference data (use default scoring)
                if (daysUntilEvent <= 3 && daysUntilEvent >= 0)
                {
                    score += 7;
                    Console.WriteLine($"Event '{ev.Title}' is within 3 days (default scoring): +7");
                }
                else if (daysUntilEvent <= 7 && daysUntilEvent > 3)
                {
                    score += 5;
                    Console.WriteLine($"Event '{ev.Title}' is within 7 days (default scoring): +5");
                }
                else if (daysUntilEvent <= 14 && daysUntilEvent > 7)
                {
                    score += 3;
                    Console.WriteLine($"Event '{ev.Title}' is within 14 days (default scoring): +3");
                }
                else if (daysUntilEvent <= 30 && daysUntilEvent > 14)
                {
                    score += 1;
                    Console.WriteLine($"Event '{ev.Title}' is within 30 days (default scoring): +1");
                }
            }

            return score;
        }

        // HELPER METHODS

        // Convert month number to name
        private string GetMonthName(int month)
        {
            return System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
        }

        // Increment dictionary value
        private void IncrementDictionary<T>(Dictionary<T, int> dict, T key)
        {
            if (dict.ContainsKey(key))
            {
                dict[key]++;
            }
            else
            {
                dict[key] = 1;
            }
        }
        //------------------------------------------------------------------------------------------------------------------------------------------//
    }
}
//--------------------------------------------------------X END OF FILE X-------------------------------------------------------------------//