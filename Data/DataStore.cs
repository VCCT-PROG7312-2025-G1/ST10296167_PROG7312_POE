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

        // Constructor
        //------------------------------------------------------------------------------------------------------------------------------------------//
        public DataStore()
        {
            _uploadFolder = Path.Combine(Path.GetTempPath(), "MyAppFiles");
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

        // Clear all uploaded files from the temporary directory
        public void ClearUploadedFiles()
        {
            if (Directory.Exists(_uploadFolder))
            {
                try
                {
                    Directory.Delete(_uploadFolder, true);
                    Directory.CreateDirectory(_uploadFolder); 
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to clear upload folder: {ex.Message}");
                }
            }
               
        }
        //------------------------------------------------------------------------------------------------------------------------------------------//
    }
}
//--------------------------------------------------------X END OF FILE X-------------------------------------------------------------------//