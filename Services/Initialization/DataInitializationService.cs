using ST10296167_PROG7312_POE.Data;
using ST10296167_PROG7312_POE.Models;
using ST10296167_PROG7312_POE.Repository.Issue;
using ST10296167_PROG7312_POE.Repository.Feedback;
using ST10296167_PROG7312_POE.Repository.Event;
using ST10296167_PROG7312_POE.Repository.Announcement;

namespace ST10296167_PROG7312_POE.Services.Initialization
{
    public class DataInitializationService
    {
    private readonly DataStore _dataStore;
    private readonly IIssueRepository _issueRepository;
    private readonly IFeedbackRepository _feedbackRepository;
    private readonly IEventRepository _eventRepository;
    private readonly IAnnouncementRepository _announcementRepository;

        // Constructor
        //------------------------------------------------------------------------------------------------------------------------------------------//
        public DataInitializationService(
            DataStore dataStore,
            IIssueRepository issueRepository,
            IFeedbackRepository feedbackRepository,
            IEventRepository eventRepository,
            IAnnouncementRepository announcementRepository)
        {
            _dataStore = dataStore;
            _issueRepository = issueRepository;
            _feedbackRepository = feedbackRepository;
            _eventRepository = eventRepository;
            _announcementRepository = announcementRepository;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------//

        // Methods
        //------------------------------------------------------------------------------------------------------------------------------------------//
        // Initialize data structures with data from the database
        public async Task InitializeDataStructuresAsync()
        {
            try
            {
                Console.WriteLine("Initializing data structures from database...");

                // Clear existing data structures
                _dataStore.ReportedIssues.Clear();
                _dataStore.UserFeedback.Clear();
                _dataStore.EventsByDate.Clear();
                _dataStore.RecentAnnouncements.Clear();

                // Load all events from database
                var dbEvents = await _eventRepository.GetAllEventsAsync();
                foreach (var ev in dbEvents)
                {
                    // Use DateTime for key (combine Date and Time)
                    var eventDateTime = ev.Date.ToDateTime(ev.Time);
                    if (!_dataStore.EventsByDate.ContainsKey(eventDateTime))
                        _dataStore.EventsByDate[eventDateTime] = new List<Models.Event>();
                    _dataStore.EventsByDate[eventDateTime].Add(ev);
                }

                // Load all announcements from database
                var dbAnnouncements = await _announcementRepository.GetAllAnnouncementsAsync();
                foreach (var ann in dbAnnouncements)
                {
                    _dataStore.RecentAnnouncements.Push(ann);
                }

                // Load all issues from database
                var dbIssues = await _issueRepository.GetAllIssuesAsync();
                
                foreach (var dbIssue in dbIssues)
                {
                    // Create a new issue for the in-memory structure
                    var issue = new Issue
                    { 
                        ID = dbIssue.ID,
                        Address = dbIssue.Address,
                        Suburb = dbIssue.Suburb,
                        Location = dbIssue.Location,
                        Category = dbIssue.Category,
                        Description = dbIssue.Description,
                        Status = dbIssue.Status, 
                        CreatedAt = dbIssue.CreatedAt,
                        Files = new LinkedList<UploadedFile>()
                    };

                    // Load files for this issue and add to LinkedList
                    var files = await _issueRepository.GetFilesByIssueIdAsync(dbIssue.ID);
                    foreach (var file in files)
                    {
                        issue.Files.AddLast(file);
                    }

                    // Add to Dictionary
                    _dataStore.ReportedIssues.Add(issue.ID, issue);
                }

                if (_dataStore.ReportedIssues.Count > 0)
                {
                    // Build AVL Tree and Graph from reported issues
                    _dataStore.ReportsAVLTree.Build(_dataStore.ReportedIssues.Values);
                    _dataStore.ReportsGraph.Build(_dataStore.ReportedIssues.Values);
                }

                // Load all feedback from database
                var dbFeedback = await _feedbackRepository.GetAllFeedbackAsync();
                foreach (var feedback in dbFeedback)
                {
                    _dataStore.UserFeedback.AddLast(feedback);
                }

                if (_dataStore.ReportedIssues.Count == 0 && _dataStore.UserFeedback.Count == 0 && _dataStore.EventsByDate.Count == 0 && _dataStore.RecentAnnouncements.Count == 0)
                {
                    Console.WriteLine("No data found in database. Data structures initialized as empty.");
                }
                else
                {
                    Console.WriteLine($"Loaded {_dataStore.ReportedIssues.Count} issues, {_dataStore.UserFeedback.Count} feedback, {_dataStore.EventsByDate.Count} events, and {_dataStore.RecentAnnouncements.Count} announcements from database.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing data structures: {ex.Message}");
                throw;
            }
        }
        //------------------------------------------------------------------------------------------------------------------------------------------//
    }
}
