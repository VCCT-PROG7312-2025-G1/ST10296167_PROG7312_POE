using ST10296167_PROG7312_POE.Models;
using IssueModel = ST10296167_PROG7312_POE.Models.Issue;
using UploadedFileModel = ST10296167_PROG7312_POE.Models.UploadedFile;

namespace ST10296167_PROG7312_POE.Repository.Issue
{
    public interface IIssueRepository
    {
        Task<IssueModel> AddIssueAsync(IssueModel issue);
        Task<IssueModel> UpdateIssueAsync(IssueModel issue);
        Task<List<IssueModel>> GetAllIssuesAsync();
        Task<UploadedFileModel> AddFileAsync(UploadedFileModel file);
        Task<List<UploadedFileModel>> GetFilesByIssueIdAsync(int issueId);
    }
}
//--------------------------------------------------------X END OF FILE X-------------------------------------------------------------------//