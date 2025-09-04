using ST10296167_PROG7312_POE.Models;

namespace ST10296167_PROG7312_POE.Services.Report
{
    public interface IReportService
    {
        Task<bool> AddIssueAsync(Models.Issue issue, IFormFile[]? files);
        void SaveFeedback(int rating, string? feedback);

        Dictionary<int, Models.Issue> GetAllIssues();

        UploadedFile? GetFileById(int fileId);

    }
}
