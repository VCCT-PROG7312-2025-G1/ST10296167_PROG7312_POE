using ST10296167_PROG7312_POE.Data;
using ST10296167_PROG7312_POE.Models;
using System.ComponentModel;

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