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
            ClearUploadedFiles();
            Directory.CreateDirectory(_uploadFolder);
            SeedData();
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

        // Seed the datastore with sample issues and files for demonstration purposes 
        public void SeedData()
        {
            try
            {
                var issues = new List<Issue>
        {
            new Issue
            {
                ID = GenerateIssueID(),
                Category = "Water & Sanitation",
                Description = "Large pothole causing damage to vehicles and creating safety hazards for pedestrians.",
                Address = "123 Main Street",
                Suburb = "Hillcrest",
                Location = "123 Main Street, Hillcrest",
                Files = new LinkedList<UploadedFile>()
            },
            new Issue
            {
                ID = GenerateIssueID(),
                Category = "Roads & Transport",
                Description = "Streetlight has been out for over two weeks, creating unsafe conditions at night.",
                Address = "456 Oak Avenue",
                Suburb = "Westville",
                Location = "456 Oak Avenue, Westville",
                Files = new LinkedList<UploadedFile>()
            },
            new Issue
            {
                ID = GenerateIssueID(),
                Category = "Public Safety",
                Description = "Burst water pipe flooding the street and causing water wastage.",
                Address = "789 Pine Road",
                Suburb = "Durban North",
                Location = "789 Pine Road, Durban North",
                Files = new LinkedList<UploadedFile>()
            },
            new Issue
            {
                ID = GenerateIssueID(),
                Category = "Waste Management",
                Description = "Illegal dumping site with household waste and construction debris.",
                Address = "321 Cedar Street",
                Suburb = "Pinetown",
                Location = "321 Cedar Street, Pinetown",
                Files = new LinkedList<UploadedFile>()
            },
            new Issue
            {
                ID = GenerateIssueID(),
                Category = "Parks & Recreation",
                Description = "Playground equipment is damaged and unsafe for children to use.",
                Address = "654 Maple Drive",
                Suburb = "Kloof",
                Location = "654 Maple Drive, Kloof",
                Files = new LinkedList<UploadedFile>()
            }
        };

                // Define sample files from wwwroot (these need to exist in your project)
                var sampleFiles = new List<(string fileName, string wwwrootPath, string mimeType)>
        {
            ("pothole1.jpg", "sample/images/pothole1.jpg", "image/jpeg"),
            ("pothole2.jpg", "sample/images/pothole2.jpg", "image/jpeg"),
            ("streetlight.jpg", "sample/images/streetlight.jpeg", "image/jpeg"),
            ("waterpipe.jpg", "sample/images/waterpipe.jpg", "image/jpeg"),
            ("dumping.jpg", "sample/images/dumping.jpg", "image/jpeg"),
            ("playground.jpg", "sample/images/playground.jpeg", "image/jpeg"),
        };

                // Copy files from wwwroot to temp directory and create UploadedFile objects
                var copiedFiles = new List<UploadedFile>();

                foreach (var sampleFile in sampleFiles)
                {
                    var copiedFile = MoveFileToTemp(sampleFile.fileName, sampleFile.wwwrootPath, sampleFile.mimeType);
                    if (copiedFile != null)
                    {
                        copiedFiles.Add(copiedFile);
                    }
                }

                // Add files to issues 
                if (copiedFiles.Count >= 6)
                {
                    copiedFiles[0].IssueID = issues[0].ID;
                    issues[0].Files.AddLast(copiedFiles[0]);

                    copiedFiles[1].IssueID = issues[0].ID;
                    issues[0].Files.AddLast(copiedFiles[1]);

                    copiedFiles[2].IssueID = issues[1].ID;
                    issues[1].Files.AddLast(copiedFiles[2]);

                    copiedFiles[3].IssueID = issues[2].ID;
                    issues[2].Files.AddLast(copiedFiles[3]);

                    copiedFiles[4].IssueID = issues[3].ID;
                    issues[3].Files.AddLast(copiedFiles[4]);

                    if (copiedFiles.Count > 6)
                    {
                        copiedFiles[6].IssueID = issues[3].ID;
                        issues[3].Files.AddLast(copiedFiles[6]);
                    }

                    copiedFiles[5].IssueID = issues[4].ID;
                    issues[4].Files.AddLast(copiedFiles[5]);
                }

                // Add all issues to the dictionary
                foreach (var issue in issues)
                {
                    ReportedIssues.Add(issue.ID, issue);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error seeding data: {ex.Message}");
            }
        }

        private UploadedFile? MoveFileToTemp(string fileName, string wwwrootRelativePath, string mimeType)
        {
            try
            {
                string currentDirectory = Directory.GetCurrentDirectory();
                string wwwrootPath = Path.Combine(currentDirectory, "wwwroot");
                string sourceFilePath = Path.Combine(wwwrootPath, wwwrootRelativePath.Replace('/', Path.DirectorySeparatorChar));

                // Check if source file exists
                if (!File.Exists(sourceFilePath))
                {
                    Console.WriteLine($"Sample file not found: {sourceFilePath}");
                    return null;
                }

                // Create unique filename for temp directory
                string uniqueFileName = Guid.NewGuid() + Path.GetExtension(fileName);
                string destinationPath = Path.Combine(_uploadFolder, uniqueFileName);

                // Copy file to temp directory
                File.Copy(sourceFilePath, destinationPath);

                // Get file size
                var fileInfo = new FileInfo(destinationPath);

                var uploadedFile = new UploadedFile
                {
                    ID = GenerateFileID(),
                    FileName = fileName,
                    MimeType = mimeType,
                    Size = fileInfo.Length,
                    FilePath = destinationPath
                };

                Console.WriteLine($"Copied sample file: {fileName} -> {destinationPath}");
                return uploadedFile;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error copying file {fileName}: {ex.Message}");
                return null;
            }
        }
        //------------------------------------------------------------------------------------------------------------------------------------------//
    }
}
//--------------------------------------------------------X END OF FILE X-------------------------------------------------------------------//