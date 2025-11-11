using Microsoft.AspNetCore.Routing.Matching;
using ST10296167_PROG7312_POE.Data;
using ST10296167_PROG7312_POE.Models;
using ST10296167_PROG7312_POE.Repository.Feedback;
using ST10296167_PROG7312_POE.Repository.Issue;

namespace ST10296167_PROG7312_POE.Services.Report
{
    public class ReportService : IReportService
    {
        private readonly DataStore _dataStore;
        private readonly IIssueRepository _issueRepository;
        private readonly IFeedbackRepository _feedbackRepository;

        // Constructor
        //------------------------------------------------------------------------------------------------------------------------------------------//
        public ReportService(DataStore dataStore, IIssueRepository issueRepository, IFeedbackRepository feedbackRepository)
        {
            _dataStore = dataStore;
            _issueRepository = issueRepository;
            _feedbackRepository = feedbackRepository;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------//

        // Methods
        //------------------------------------------------------------------------------------------------------------------------------------------//
        // Add the issue (along with associated files) to the datastore reported issues dictionary
        public async Task<bool> AddIssueAsync(Issue issue, IFormFile[]? files)
        {
            try
            {
                // Set location and save to database first
                issue.Location = $"{issue.Address}, {issue.Suburb}";
                var savedIssue = await _issueRepository.AddIssueAsync(issue);
                
                // Update the issue with the database-generated ID
                issue.ID = savedIssue.ID;

                if(files != null)
                {
                    foreach (var file in files)
                    {
                        try
                        {
                            string filePath = await SaveFileToDisk(file);
                            Console.WriteLine($"File saved: {filePath}");

                            UploadedFile uploadedFile = new UploadedFile
                            {
                                FileName = file.FileName,
                                MimeType = file.ContentType,
                                Size = file.Length,
                                FilePath = filePath,
                                IssueID = issue.ID,
                            };
                            
                            // Save file to database
                            var savedFile = await _issueRepository.AddFileAsync(uploadedFile);
                            uploadedFile.ID = savedFile.ID;
                            
                            // Add to in-memory data structure
                            issue.Files?.AddLast(uploadedFile);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error saving file '{file.FileName}': {ex.Message}");
                        }
                    }
                }

                // Add to in-memory data structure
                _dataStore.ReportedIssues.Add(issue.ID, issue);
                Console.WriteLine("SAVED ISSUE");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public Dictionary<int, Issue> GetAllIssues()
        {
            return _dataStore.ReportedIssues;
        }

        // Save a uploaded file to the uploads directory and return the file path
        public async Task<string> SaveFileToDisk(IFormFile file)
        {
            try
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                string fullPath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return fullPath;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving file to disk: {ex.Message}");
                throw;
            }
        }

        // Save submitted user feedback to the datastore in a Feedback object
        public async Task SaveFeedback(int rating, string? feedback)
        {
            Feedback userFeedback = new Feedback
            {
                Rating = rating,
                OptionalFeedback = feedback
            };
            
            // Save to database first
            var savedFeedback = await _feedbackRepository.AddFeedbackAsync(userFeedback);
            userFeedback.ID = savedFeedback.ID;
            
            // Add to in-memory data structure
            _dataStore.UserFeedback.AddLast(userFeedback);
            Console.WriteLine($"User Rating: {rating} stars");

            if (!string.IsNullOrEmpty(feedback))
            {
                Console.WriteLine($"User Feedback: {feedback}");
            }
            Console.WriteLine("Feedback saved.");
        }

        // Retrueve all files associated with a specific issue ID
        public UploadedFile? GetFileById(int fileId)
        {
            foreach (var issue in _dataStore.ReportedIssues.Values)
            {
                if (issue.Files != null)
                {
                    foreach (var file in issue.Files)
                    {
                        if (file.ID == fileId)
                        {
                            return file;
                        }
                    }
                }
            }
            return null;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------//
        // Get issues filtered by the selected filter options and sorted by priority using an AVL tree and Min-Heap
        public List<Issue> GetFilteredAndSortedIssues(RequestStatusFilter filter)
        {
            List<Issue> issueList = new List<Issue>();

            if (filter.ID.HasValue)
            {
                // Direct AVL tree search
                var issue = GetIssueById(filter.ID.Value);

                if (issue != null)
                {
                    issueList.Add(issue);
                }
            }
            else
            {
                // Get all issues from AVL tree in sorted order
                issueList = _dataStore.ReportsAVLTree.InOrder();
            }

            // Apply additional filters
            if (!string.IsNullOrEmpty(filter.Category))
            {
                issueList = issueList.Where(i => i.Category == filter.Category).ToList();
            }

            if (filter.Status.HasValue)
            {
                issueList = issueList.Where(i => i.Status == filter.Status.Value).ToList();
            }

            if (filter.Date.HasValue)
            {
                issueList = issueList.Where(i => DateOnly.FromDateTime(i.CreatedAt) == filter.Date.Value).ToList();
            }

            // Use Min-Heap for priority based sorting
            if (issueList.Any())
            {
                _dataStore.ReportsMinHeap.Build(issueList);
                issueList = _dataStore.ReportsMinHeap.ExtractAllSorted();
            }

            return issueList;
        }

        // Get a specific issue by ID using a direct AVL tree search
        public Issue? GetIssueById(int id)
        {
            return _dataStore.ReportsAVLTree.Search(id);
        }

        public List<Issue> GetRelatedIssues(int issueId)
        {
            return _dataStore.ReportsGraph.GetRelatedReports(issueId);
        }

        public int GetRelatedCount(int issueId)
        {
            return _dataStore.ReportsGraph.GetRelatedCount(issueId);
        }

        public List<IssueCluster> GetIssueClusters()
        {
            return new List<IssueCluster>();
        }

        public async Task<bool> UpdateIssueStatusAsync(int issueId, IssueStatus newStatus)
        {
            return new bool();
        }

        public RequestStatusStats GetStatistics()
        {
            return new RequestStatusStats();
        }
    }
}
//--------------------------------------------------------X END OF FILE X-------------------------------------------------------------------//