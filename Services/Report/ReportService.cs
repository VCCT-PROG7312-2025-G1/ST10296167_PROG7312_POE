using ST10296167_PROG7312_POE.Data;
using ST10296167_PROG7312_POE.Models;

namespace ST10296167_PROG7312_POE.Services.Report
{
    public class ReportService : IReportService
    {
        private readonly DataStore _dataStore;

        // Constrcutor
        //------------------------------------------------------------------------------------------------------------------------------------------//
        public ReportService(DataStore dataStore)
        {
            _dataStore = dataStore;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------//

        // Methods
        //------------------------------------------------------------------------------------------------------------------------------------------//
        // Add the issue (along with associated files) to the datastore reported issues dictionary
        public async Task<bool> AddIssueAsync(Issue issue, IFormFile[]? files)
        {
            try
            {
                issue.ID = _dataStore.GenerateIssueID();
                Console.WriteLine(issue.ID);
                issue.Location = $"{issue.Address}, {issue.Suburb}";
                Console.WriteLine(issue.Location);

                if(files != null)
                {
                    foreach (var file in files)
                    {
                        string filePath = await SaveFileToDisk(file);
                        Console.WriteLine($"File saved: {filePath}");

                        UploadedFile uploadedFile = new UploadedFile
                        {
                            ID = _dataStore.GenerateFileID(),
                            FileName = file.FileName,
                            MimeType = file.ContentType,
                            Size = file.Length,
                            FilePath = filePath,
                            IssueID = issue.ID,
                        };
                        issue.Files?.AddLast(uploadedFile);
                    }
                }

                _dataStore.ReportedIssues.Add(issue.ID, issue);
                Console.WriteLine("SAVED ISSUE");
                testStore();
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

        // Save a uploaded file to a temporary directory and return the file path
        public async Task<string> SaveFileToDisk(IFormFile file)
        {
            string runtimeFolder = Path.Combine(Path.GetTempPath(), "MyAppFiles");
            Directory.CreateDirectory(runtimeFolder);

            string uniqueFileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            string fullPath = Path.Combine(runtimeFolder, uniqueFileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return fullPath;
        }

        // Save submitted user feedback to the datastore in a Feedback object
        public void SaveFeedback(int rating, string? feedback)
        {
            Feedback userFeedback = new Feedback
            {
                Rating = rating,
                OptionalFeedback = feedback
            };
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


        // TESTING 
        public void testStore()
        {
            foreach (var repIssue in _dataStore.ReportedIssues)
            {
                Console.WriteLine($"ID: {repIssue.Value.ID}, Location: {repIssue.Value.Location}, Description: {repIssue.Value.Description}");
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------//
    }
}
//--------------------------------------------------------X END OF FILE X-------------------------------------------------------------------//