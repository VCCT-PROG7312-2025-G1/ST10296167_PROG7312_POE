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