using ST10296167_PROG7312_POE.Models;

namespace ST10296167_PROG7312_POE.Data
{
    public class DataStore
    {
        private readonly string _uploadFolder;
        private int nextIssueId = 1;
        private int nextFileId = 1;
        public Dictionary<int, Issue> ReportedIssues { get; set; } = new Dictionary<int, Issue>();
        public LinkedList<Feedback> UserFeedback { get; set; } = new LinkedList<Feedback>();
        public SortedDictionary<DateTime, List<Event>> EventsByDate { get; set; } = new SortedDictionary<DateTime, List<Event>>();
        public HashSet<string> UniqueCategories { get; set; } = new HashSet<string>();
        public Stack<Announcement> RecentAnnouncements { get; set; } = new Stack<Announcement>();
        public Queue<SearchQuery> SearchHistory { get; set; } = new Queue<SearchQuery>();

        // Constructor
        //------------------------------------------------------------------------------------------------------------------------------------------//
        public DataStore()
        {
            _uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            Directory.CreateDirectory(_uploadFolder);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------//

        // Methods
        //------------------------------------------------------------------------------------------------------------------------------------------//
        public void LogSearch(SearchQuery search)
        {
            // Ensure valid timestamp
            search.SearchTimestamp = search.SearchTimestamp == default
                ? DateTime.UtcNow
                : search.SearchTimestamp;

            SearchHistory.Enqueue(search);
            Console.WriteLine("Added to search to search history");

            // Limit queue size
            if(SearchHistory.Count > 10)
            {
                SearchHistory.Dequeue();
            }
        }

        public int GenerateIssueID()
        {
            return nextIssueId++;
        }

        public int GenerateFileID()
        {
            return nextFileId++;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------//
    }
}
//--------------------------------------------------------X END OF FILE X-------------------------------------------------------------------//