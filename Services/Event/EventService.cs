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

        // Generate event recommendations based on search history
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

                // Get top 3 highest scoring events
                var recommendedEvents = scoredEvents
                    .Where(x => x.Score > 0) 
                    .OrderByDescending(x => x.Score)
                    .ThenBy(x => x.Event.Date)
                    .ThenBy(x => x.Event.Time)
                    .Select(x => x.Event)
                    .Take(3) 
                    .ToList();

                Console.WriteLine($"Generated {recommendedEvents.Count} recommendations based on search history");

                // Save to list
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

        // Analyze search patterns from search history to determine user preferences for event recommendations
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

                // Analyze start date
                if (search.StartDate.HasValue)
                {
                    int month = search.StartDate.Value.Month;
                    IncrementDictionary(analysis.MonthPreferences, month);
                }

                // Analyze end date
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
                    // Assume mid-term if only one date is searched
                    IncrementDictionary(analysis.DayRangePreferences, "mid-term");
                }
            }

            Console.WriteLine($"Search pattern analysis: {analysis.ToString()}");

            return analysis;
        }

        // Calculate score
        // I used Claude to help me brainstorm & design this recommendation algorithm using a point system
        private int CalculateEventScore(EventModel ev, SearchPatternAnalysis analysis)
        {
            int score = 0;
            var today = DateTime.Now;
            var eventDateTime = ev.Date.ToDateTime(ev.Time);

            // CATEGORY SCORING (0-10 points)
            score += CategoryScore(ev, analysis);

            // MONTH PREFERENCE SCORING (0-4 points)
            score += MonthScore(ev, analysis);

            // DAY RANGE / PROXIMITY SCORING (0-4 points)

            // Calculate days between today and event date
            int daysUntilEvent = (int)(eventDateTime.Date - today.Date).TotalDays;

            // Determine user's preferred search range
            string? userPreferredRange = analysis.GetPreferredRange();

            if (userPreferredRange == "near-term")
            {
                // User prefers near-term events (1 week)
                score += NearTermScore(ev.Title, daysUntilEvent);
            }
            else if (userPreferredRange == "mid-term")
            {
                // User prefers mid-term events (1-4 weeks)
                score += MidTermScore(ev.Title, daysUntilEvent);
            }
            else if (userPreferredRange == "far-term")
            {
                // User prefers far-term events (1+ months)
                score += FarTermScore(ev.Title, daysUntilEvent);
            }
            else
            {
                // Default scoring
                score += DefaultScore(ev.Title, daysUntilEvent);
            }

            return score;
        }

        // Determine category score (0-10 points)
        private int CategoryScore(EventModel ev, SearchPatternAnalysis analysis)
        {
            if (analysis.CategoryFrequency.Count > 0)
            {
                // Get list of categories by frequency
                var sortedCategories = analysis.CategoryFrequency
                    .OrderByDescending(x => x.Value)
                    .Select(x => x.Key)
                    .ToList();

                // Check if event category matches user's top categories
                if (sortedCategories.Count > 0 && ev.Category == sortedCategories[0])
                {
                    // most searched category = 10 points
                    Console.WriteLine($"Event '{ev.Title}' matched top category '{ev.Category}': +10");
                    return 10;
                }
                else if (sortedCategories.Count > 1 && ev.Category == sortedCategories[1])
                {
                    // 2ND most searched category = 8 points
                    Console.WriteLine($"Event '{ev.Title}' matched 2nd category '{ev.Category}': +8");
                    return 8;
                }
                else if (sortedCategories.Count > 2 && ev.Category == sortedCategories[2])
                {
                    // 3RD most searched category = 6 points
                    Console.WriteLine($"Event '{ev.Title}' matched 3rd category '{ev.Category}': +6");
                    return 6;
                }
                else if (analysis.AllSearchedCategories.Contains(ev.Category))
                {
                    // Category appears in search history but not top 3 = 4 points
                    Console.WriteLine($"Event '{ev.Title}' matched searched category '{ev.Category}': +4");
                    return 4;
                }
            }

            return 0;
        }

        // Determine month preference score (0-4 points)
        private int MonthScore(EventModel ev, SearchPatternAnalysis analysis)
        {
            int eventMonth = ev.Date.Month;

            if (analysis.MonthPreferences.Count > 0)
            {
                // Get the most frequently searched month
                var topMonth = analysis.MonthPreferences.OrderByDescending(x => x.Value).First().Key;

                if (eventMonth == topMonth)
                {
                    // Event in most-searched month = 4 points
                    Console.WriteLine($"Event '{ev.Title}' is in top month ({GetMonthName(eventMonth)}): +4");
                    return 4;
                }
                else if (analysis.MonthPreferences.ContainsKey(eventMonth))
                {
                    // Event in a month that was searched = 2 points
                    Console.WriteLine($"Event '{ev.Title}' is in searched month ({GetMonthName(eventMonth)}): +2");
                    return 2;
                }
            }

            return 0;
        }

        // Determine near-term score
        private int NearTermScore(string eventTitle, int daysUntilEvent)
        {
            if (daysUntilEvent <= 3 && daysUntilEvent >= 0)
            {
                Console.WriteLine($"Event '{eventTitle}' is within 3 days (user prefers near-term): +7");
                return 4;
            }
            else if (daysUntilEvent <= 7 && daysUntilEvent > 3)
            {
                Console.WriteLine($"Event '{eventTitle}' is within 7 days (user prefers near-term): +5");
                return 3;
            }
            else if (daysUntilEvent <= 14 && daysUntilEvent > 7)
            {
                Console.WriteLine($"Event '{eventTitle}' is within 14 days (less preferred): +2");
                return 1;
            }

            // Beyond 14 days = no points
            return 0;
        }

        // Determine mid-term score
        private int MidTermScore(string eventTitle, int daysUntilEvent)
        {
            if (daysUntilEvent <= 7 && daysUntilEvent >= 0)
            {
                Console.WriteLine($"Event '{eventTitle}' is within 7 days (user prefers mid-term): +4");
                return 3;
            }
            else if (daysUntilEvent <= 14 && daysUntilEvent > 7)
            {
                Console.WriteLine($"Event '{eventTitle}' is within 14 days (user prefers mid-term): +6");
                return 4;
            }
            else if (daysUntilEvent <= 30 && daysUntilEvent > 14)
            {
                Console.WriteLine($"Event '{eventTitle}' is within 30 days (user prefers mid-term): +4");
                return 2;
            }
            else if (daysUntilEvent <= 60 && daysUntilEvent > 30)
            {
                Console.WriteLine($"Event '{eventTitle}' is within 60 days (less preferred): +1");
                return 1;
            }

            return 0;
        }

        // Determine far-term preference score
        private int FarTermScore(string eventTitle, int daysUntilEvent)
        {
            if (daysUntilEvent <= 14 && daysUntilEvent >= 0)
            {
                Console.WriteLine($"Event '{eventTitle}' is within 14 days (user prefers far-term): +2");
                return 1;
            }
            else if (daysUntilEvent <= 30 && daysUntilEvent > 14)
            {
                Console.WriteLine($"Event '{eventTitle}' is within 30 days (user prefers far-term): +4");
                return 2;
            }
            else if (daysUntilEvent <= 60 && daysUntilEvent > 30)
            {
                Console.WriteLine($"Event '{eventTitle}' is within 60 days (user prefers far-term): +6");
                return 4;
            }
            else if (daysUntilEvent > 60)
            {
                Console.WriteLine($"Event '{eventTitle}' is 60+ days (user prefers far-term): +4");
                return 2;
            }

            return 0;
        }

        // Determine default score
        private int DefaultScore(string eventTitle, int daysUntilEvent)
        {
            if (daysUntilEvent <= 3 && daysUntilEvent >= 0)
            {
                Console.WriteLine($"Event '{eventTitle}' is within 3 days (default scoring): +7");
                return 4;
            }
            else if (daysUntilEvent <= 7 && daysUntilEvent > 3)
            {
                Console.WriteLine($"Event '{eventTitle}' is within 7 days (default scoring): +5");
                return 3;
            }
            else if (daysUntilEvent <= 14 && daysUntilEvent > 7)
            {
                Console.WriteLine($"Event '{eventTitle}' is within 14 days (default scoring): +3");
                return 2;
            }
            else if (daysUntilEvent <= 30 && daysUntilEvent > 14)
            {
                Console.WriteLine($"Event '{eventTitle}' is within 30 days (default scoring): +1");
                return 1;
            }

            return 0;
        }

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