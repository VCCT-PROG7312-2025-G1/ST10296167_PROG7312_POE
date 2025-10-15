using ST10296167_PROG7312_POE.Models;
using EventModel = ST10296167_PROG7312_POE.Models.Event;

namespace ST10296167_PROG7312_POE.Services.Recommendation
{
    public class RecommendationsGenerator
    {

        // Constructor
        // -----------------------------------------------------------------------------------------------------------------------------------------//
        public RecommendationsGenerator() 
        { 

        }
        // -----------------------------------------------------------------------------------------------------------------------------------------//

        // Methods
        // -----------------------------------------------------------------------------------------------------------------------------------------//
        // Generate event recommendations based on search history
        public List<EventModel> GenerateRecommendations(Queue<SearchQuery> searchHistory, List<EventModel> eventList, int maxRecommendations = 3)
        {
            try
            {
                // Check search history
                if (searchHistory == null || searchHistory.Count == 0)
                {
                    Console.WriteLine("No search history");
                    return new List<EventModel>();
                }

                // Check events
                if (eventList == null || eventList.Count == 0)
                {
                    Console.WriteLine("No events available for recommendations");
                    return new List<EventModel>();
                }

                // Analyze search patterns to get user preferences
                var analysis = AnalyzeSearchPatterns(searchHistory);

                // Score each event
                var scoredEvents = new List<(EventModel Event, double Score)>();

                foreach (var ev in eventList)
                {
                    double score = CalculateEventScore(ev, analysis);
                    scoredEvents.Add((ev, score));
                }

                // Return top recommendations
                var recommendations = scoredEvents
                    .Where(x => x.Score > 0)
                    .OrderByDescending(x => x.Score)
                    .ThenBy(x => x.Event.Date)
                    .ThenBy(x => x.Event.Time)
                    .Take(maxRecommendations)
                    .Select(x => x.Event)
                    .ToList();

                Console.WriteLine($"Generated {recommendations.Count} recommendations");

                return recommendations;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating recommendations: {ex.Message}");
                return new List<EventModel>();
            }
        }

        // Analyzes search patterns from search history to identify category frequencies and date range preferences, applying weighting to more recent searches.
        private SearchPatternAnalysis AnalyzeSearchPatterns(Queue<SearchQuery> searchHistory)
        {
            var analysis = new SearchPatternAnalysis();
            var searchList = searchHistory.ToList();

            Console.WriteLine($"Analyzing {searchList.Count} searches");

            // Get each search's recency weighting
            for (int i = 0; i < searchList.Count; i++)
            {
                var search = searchList[i];
                int positionFromEnd = searchList.Count - 1 - i; 
                double recencyWeight = GetRecencyWeight(positionFromEnd);

                // CATEGORY ANALYSIS

                if (!string.IsNullOrEmpty(search.Category))
                {
                    // Add category multiple times based on recency weight
                    int weightedCount = (int)Math.Ceiling(recencyWeight * 10);
                    for (int w = 0; w < weightedCount; w++)
                    {
                        IncrementDictionary(analysis.CategoryFrequency, search.Category);
                    }
                    // Add to model
                    analysis.AllSearchedCategories.Add(search.Category);
                    Console.WriteLine($"Category: '{search.Category}' (weighted {weightedCount}x) ");
                }
                else
                {
                    Console.WriteLine("Category: (none) ");
                }

                // DATE RANGE ANALYSIS

                if (search.StartDate.HasValue || search.EndDate.HasValue)
                {
                    // Convert to date only
                    DateOnly start = search.StartDate ?? DateOnly.FromDateTime(DateTime.Today);
                    DateOnly end = search.EndDate ?? start.AddDays(90);

                    // Add to model
                    analysis.SearchDateRanges.Add((start, end, recencyWeight));
                    Console.WriteLine($"Date range: {start:MMM dd} to {end:MMM dd} (weight: {recencyWeight:F1}) ");
                }
                else
                {
                    Console.WriteLine("Date range: (none) ");
                }
            }

            Console.WriteLine($"\n=== Analysis Summary ===");
            Console.WriteLine(analysis.ToString());

            return analysis;
        }

        // Calculate's an events overall score based on category relevance, proximity to searched ranges, and recency weighting.
        // I used Claude to help me brainstorm & design this recommendation algorithm using a point system combined wiht recency weighting for more responsive recommendations
        private double CalculateEventScore(EventModel ev, SearchPatternAnalysis analysis)
        {
            double score = 0;
            var eventDate = ev.Date;

            Console.WriteLine($"Scoring: '{ev.Title}' ({eventDate:MMM dd}, {ev.Category}) ");

            // Date Window Score (0-10 points, weighted by recency)
            double dateScore = DateWindowScore(eventDate, analysis);

            // Category Score (0-15 points, heavily weighted)
            double categoryScore = CategoryScore(ev.Category, analysis);

            // Date Range Priority Score
            if (analysis.SearchDateRanges.Count > 0)
            {
                if (dateScore > 0)
                {
                    // Event is within or close to searched date ranges
                    score = dateScore + categoryScore;
                }
                else
                {
                    // Event is outside all searched ranges
                    score = categoryScore * 0.3; // Reduce category score for far-away events
                    if (score > 0)
                        Console.WriteLine($"Category reduced to: +{score:F1} (outside searched date ranges) ");
                }
            }
            else
            {
                // Use proximity bonus if no date ranges searched
                int daysUntilEvent = (eventDate.DayNumber - DateOnly.FromDateTime(DateTime.Today).DayNumber);
                double proximityScore = ProximityBonus(daysUntilEvent);
                score = categoryScore + proximityScore;
                Console.WriteLine($"Proximity: +{proximityScore:F1} ");
            }

            Console.WriteLine($"TOTAL SCORE: {score:F1} \n");
            return score;
        }

        // Determine category score based on it's frequency in recent searches (0-15 points)
        private double CategoryScore(string eventCategory, SearchPatternAnalysis analysis)
        {
            // Check for searches
            if (analysis.CategoryFrequency.Count == 0)
            {
                Console.WriteLine("Category: 0 (no category searches) ");
                return 0;
            }

            // Get list of categories by frequency
            var sortedCategories = analysis.CategoryFrequency
                .OrderByDescending(x => x.Value)
                .ToList();

            // Find where this category ranks
            for (int i = 0; i < sortedCategories.Count; i++)
            {
                if (sortedCategories[i].Key == eventCategory)
                {
                    double score;

                    switch (i)
                    {
                        // Most searched category (desc)
                        case 0:
                            score = 15; 
                            break;
                        case 1:
                            score = 12;
                            break;
                        case 2:
                            score = 9;
                            break;
                        default:
                            score = 6;
                            break;
                    }
                    Console.WriteLine($"Category: +{score} (rank #{i + 1}, '{eventCategory}') ");
                    return score;
                }
            }

            Console.WriteLine($"Category: 0 ('{eventCategory}' not searched) ");

            return 0;
        }

        // Determinet the proximity of an event date to searched ranges, assigning higher scores to matches and nearby dates.
        private double DateWindowScore(DateOnly eventDate, SearchPatternAnalysis analysis)
        {
            // Check for searches
            if (analysis.SearchDateRanges.Count == 0)
            {
                Console.WriteLine("DateWindow: 0 (no date searches) ");
                return 0;
            }

            double bestScore = 0;

            foreach (var (start, end, weight) in analysis.SearchDateRanges)
            {
                double rangeScore = 0;

                // In searched range
                if (eventDate >= start && eventDate <= end)
                {
                    rangeScore = 10 * weight; // Score weighed by recency 
                    Console.WriteLine($"DateWindow: +{rangeScore:F1} (in range {start:MMM dd}-{end:MMM dd}, weight {weight:F1}x) ");
                }
                // Close to searched range
                else
                {
                    int daysFromRange;
                    if (eventDate < start)
                    {
                        daysFromRange = (start.DayNumber - eventDate.DayNumber);
                    }
                    else
                    {
                        daysFromRange = (eventDate.DayNumber - end.DayNumber);
                    }

                    // Range falloff
                    if (daysFromRange <= 3)
                    {
                        rangeScore = 4 * weight;  // Within 3 days gets base 4 points
                    }
                    else if (daysFromRange <= 7)
                    {
                        rangeScore = 2 * weight;  // Within 1 week gets base 2 points
                    }
                    else if (daysFromRange <= 14)
                    {
                        rangeScore = 1 * weight;  // Within 2 weeks gets base 1 point
                    }
                    // Beyond 2 weeks gets 0 points

                    if (rangeScore > 0)
                    {
                        Console.WriteLine($"DateWindow: +{rangeScore:F1} ({daysFromRange} days from range, weight {weight:F1}x) ");
                    }
                }

                // Determine best score
                bestScore = Math.Max(bestScore, rangeScore);
            }

            if (bestScore == 0)
            {
                Console.WriteLine("DateWindow: 0 (too far from any searched range) ");
            }

            return bestScore;
        }

        // Helper methods

        // Returns a proximity score based on how close the event is
        private double ProximityBonus(int daysUntilEvent)
        {
            if (daysUntilEvent <= 7 && daysUntilEvent >= 0)
            {
                return 3;
            }
            else if (daysUntilEvent <= 30 && daysUntilEvent > 7)
            {
                return 2;
            }
            else if (daysUntilEvent <= 60 && daysUntilEvent > 30)
            {
                return 1;
            }

            return 0;
        }

        // Get recency weight depending on position in the queue (used for score calculation) 
        private double GetRecencyWeight(int positionFromEnd)
        {
            double weight;

            switch (positionFromEnd)
            {
                // Most recent (desc)
                case 0:
                    weight = 1.0; // Full weight
                    break;
                case 1:
                    weight = 0.5;  
                    break;
                case 2:
                    weight = 0.25; 
                    break;
                case 3:
                    weight = 0.1;  
                    break;
                default:
                    weight = 0.05;
                    break;
            }

            return weight;
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