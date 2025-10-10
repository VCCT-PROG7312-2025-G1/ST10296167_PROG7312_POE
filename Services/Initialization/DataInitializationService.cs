using ST10296167_PROG7312_POE.Data;
using ST10296167_PROG7312_POE.Models;
using ST10296167_PROG7312_POE.Repository.Issue;
using ST10296167_PROG7312_POE.Repository.Feedback;

namespace ST10296167_PROG7312_POE.Services.Initialization
{
    public class DataInitializationService
    {
        private readonly DataStore _dataStore;
        private readonly IIssueRepository _issueRepository;
        private readonly IFeedbackRepository _feedbackRepository;

        // Constructor
        //------------------------------------------------------------------------------------------------------------------------------------------//
        public DataInitializationService(DataStore dataStore, IIssueRepository issueRepository, IFeedbackRepository feedbackRepository)
        {
            _dataStore = dataStore;
            _issueRepository = issueRepository;
            _feedbackRepository = feedbackRepository;
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

                // Load all feedback from database
                var dbFeedback = await _feedbackRepository.GetAllFeedbackAsync();
                foreach (var feedback in dbFeedback)
                {
                    _dataStore.UserFeedback.AddLast(feedback);
                }

                if (_dataStore.ReportedIssues.Count == 0 && _dataStore.UserFeedback.Count == 0)
                {
                    Console.WriteLine("No data found in database. Data structures initialized as empty.");
                }
                else
                {
                    Console.WriteLine($"Loaded {_dataStore.ReportedIssues.Count} issues and {_dataStore.UserFeedback.Count} feedback entries from database.");
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
