namespace ST10296167_PROG7312_POE.Services.Report
{
    public interface IReportService
    {
        Task<bool> AddIssueAsync(Models.Issue issue, IFormFile[]? files);

    }
}
