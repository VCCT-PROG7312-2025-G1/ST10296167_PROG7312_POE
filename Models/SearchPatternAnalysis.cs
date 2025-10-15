namespace ST10296167_PROG7312_POE.Models
{
    public class SearchPatternAnalysis
    {
        // Track how many times each category was searched
        public Dictionary<string, int> CategoryFrequency { get; set; } = new Dictionary<string, int>();

        // All unique categories that appear in search history
        public HashSet<string> AllSearchedCategories { get; set; } = new HashSet<string>();

        // Store date ranges from searches with recency weights
        public List<(DateOnly StartDate, DateOnly EndDate, double Weight)> SearchDateRanges { get; set; } = new List<(DateOnly, DateOnly, double)>();

        // Get the most frequently searched category, or null if no categories searched
        public string? GetTopCategory()
        {
            if (CategoryFrequency.Count == 0)
            {
                return null;
            }

            return CategoryFrequency.OrderByDescending(x => x.Value).First().Key;
        }


        // Returns a string representation of the analysis for logging/debugging (Claude helped me create this for logging / debugging).
        public override string ToString()
        {
            var parts = new List<string>();

            if (CategoryFrequency.Count > 0)
            {
                var categories = string.Join(", ", CategoryFrequency.Select(c => $"{c.Key} ({c.Value}x)"));
                parts.Add($"Categories: {categories}");
            }

            if (SearchDateRanges.Count > 0)
            {
                var ranges = string.Join(", ", SearchDateRanges.Select(r =>
                {
                    string rangeText;
                    if (r.StartDate == DateOnly.MinValue)
                        rangeText = $"until {r.EndDate:MMM dd}";
                    else if (r.EndDate == DateOnly.MaxValue)
                        rangeText = $"from {r.StartDate:MMM dd} onwards";
                    else
                        rangeText = $"{r.StartDate:MMM dd}-{r.EndDate:MMM dd}";
                    
                    return $"{rangeText} (w:{r.Weight:F1})";
                }));
                parts.Add($"Date Ranges: {ranges}");
            }

            return parts.Count > 0
                ? string.Join(" | ", parts)
                : "No search data";
        }
    }
}
//--------------------------------------------------------X END OF FILE X-------------------------------------------------------------------//