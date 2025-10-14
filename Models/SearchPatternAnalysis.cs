namespace ST10296167_PROG7312_POE.Models
{
    public class SearchPatternAnalysis
    {
        // Track how many times each category was searched
        public Dictionary<string, int> CategoryFrequency { get; set; } = new Dictionary<string, int>();

        // All unique categories that appear in search history
        public HashSet<string> AllSearchedCategories { get; set; } = new HashSet<string>();

        // Track how many times each month was searched for
        public Dictionary<int, int> MonthPreferences { get; set; } = new Dictionary<int, int>();

        // Dictionary tracking what types of date ranges the user typically searches for
        // Range types:
        // - "near-term": Searches for events within 1 week (0-7 days)
        // - "mid-term": Searches for events 1-4 weeks out (8-30 days)
        // - "far-term": Searches for events 1+ months out (31+ days)
        public Dictionary<string, int> DayRangePreferences { get; set; } = new Dictionary<string, int>();

        // Get the most frequently searched category, or null if no categories searched
        public string? GetTopCategory()
        {
            if (CategoryFrequency.Count == 0)
                return null;

            return CategoryFrequency.OrderByDescending(x => x.Value).First().Key;
        }

        // Get the most frequently searched month (1-12), or null if no months searched
        public int? GetTopMonth()
        {
            if (MonthPreferences.Count == 0)
                return null;

            return MonthPreferences.OrderByDescending(x => x.Value).First().Key;
        }

        // Get the user's preferred search range type
        public string? GetPreferredRange()
        {
            if (DayRangePreferences.Count == 0)
                return null;

            return DayRangePreferences.OrderByDescending(x => x.Value).First().Key;
        }

        // Returns a string representation of the analysis for logging/debugging (Claude helped me create this).
        public override string ToString()
        {
            var parts = new List<string>();

            if (CategoryFrequency.Count > 0)
            {
                var categories = string.Join(", ", CategoryFrequency.Select(c => $"{c.Key} ({c.Value}x)"));
                parts.Add($"Categories: {categories}");
            }

            if (MonthPreferences.Count > 0)
            {
                var months = string.Join(", ", MonthPreferences.Select(m =>
                    $"{System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(m.Key)} ({m.Value}x)"));
                parts.Add($"Months: {months}");
            }

            if (DayRangePreferences.Count > 0)
            {
                var ranges = string.Join(", ", DayRangePreferences.Select(r => $"{r.Key} ({r.Value}x)"));
                parts.Add($"Ranges: {ranges}");
            }

            return parts.Count > 0
                ? "Search Pattern Analysis: " + string.Join(" | ", parts)
                : "Search Pattern Analysis: No data";
        }
    }
}
//--------------------------------------------------------X END OF FILE X-------------------------------------------------------------------//