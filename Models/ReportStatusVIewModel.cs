namespace ST10296167_PROG7312_POE.Models
{
    public class ReportStatusVIewModel
    {
        public List<Issue> FilteredIssues { get; set; } = new List<Issue>();
        public RequestStatusFilter CurrentFilter { get; set; } = new RequestStatusFilter();
        public RequestStatusStats Statistics { get; set; } = new RequestStatusStats();
        public bool IsEmployee { get; set; }
        public Dictionary<int, int> RelatedCounts { get; set; } = new Dictionary<int, int>();
    }

    public class RequestStatusFilter
    {
        public int? ID { get; set; }
        public string? Category { get; set; }
        public IssueStatus? Status { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }

        public bool HasFilters()
        {
            return ID.HasValue || !string.IsNullOrEmpty(Category) || Status.HasValue || StartDate.HasValue || EndDate.HasValue;
        }
    }

    public class RequestStatusStats
    {
        public int TotalIssues { get; set; }
        public Dictionary<IssueStatus, int> IssuesByStatus { get; set; } = new Dictionary<IssueStatus, int>();
        public Dictionary<string, int> IssuesByCategory { get; set; } = new Dictionary<string, int>();
    }

    public class IssueDetailsViewModel
    {
        public Issue Issue { get; set; } = null!;
        public List<Issue> RelatedIssues { get; set; } = new List<Issue>();
    }
}
//--------------------------------------------------------X END OF FILE X-------------------------------------------------------------------//