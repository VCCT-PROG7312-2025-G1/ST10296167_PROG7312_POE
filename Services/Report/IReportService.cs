using ST10296167_PROG7312_POE.Models;

namespace ST10296167_PROG7312_POE.Services.Report
{
    public interface IReportService
    {
        Task<bool> AddIssueAsync(Issue issue, IFormFile[]? files);
        Task SaveFeedback(int rating, string? feedback);
        Dictionary<int, Issue> GetAllIssues();
        UploadedFile? GetFileById(int fileId);
        List<Issue> GetFilteredAndSortedIssues(RequestStatusFilter filter);
        Issue? GetIssueById(int issueId);
        List<Issue> GetRelatedIssues(int issueId);
        int GetRelatedCount(int issueId);
        Task<bool> UpdateIssueStatusAsync(int issueId, IssueStatus newStatus);
        RequestStatusStats GetStatistics();
    }
}
//--------------------------------------------------------X END OF FILE X-------------------------------------------------------------------//