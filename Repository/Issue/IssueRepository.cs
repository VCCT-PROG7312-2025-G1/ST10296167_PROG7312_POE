using Microsoft.EntityFrameworkCore;
using ST10296167_PROG7312_POE.Data;
using ST10296167_PROG7312_POE.Models;
using IssueModel = ST10296167_PROG7312_POE.Models.Issue;
using UploadedFileModel = ST10296167_PROG7312_POE.Models.UploadedFile;

namespace ST10296167_PROG7312_POE.Repository.Issue
{
    public class IssueRepository : IIssueRepository
    {
        private readonly ApplicationDbContext _context;

        // Constructor
        //------------------------------------------------------------------------------------------------------------------------------------------//
        public IssueRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------//

        // Methods
        //------------------------------------------------------------------------------------------------------------------------------------------//
        // Add a new issue to the database
        public async Task<IssueModel> AddIssueAsync(IssueModel issue)
        {
            _context.Issues.Add(issue);
            await _context.SaveChangesAsync();
            return issue;
        }

        // Get all issues from the database
        public async Task<List<IssueModel>> GetAllIssuesAsync()
        {
            return await _context.Issues
                .Include(i => i.DbFiles)
                .ToListAsync();
        }

        // Add a new file to the database
        public async Task<UploadedFileModel> AddFileAsync(UploadedFileModel file)
        {
            _context.UploadedFiles.Add(file);
            await _context.SaveChangesAsync();
            return file;
        }

        // Get all files for a specific issue
        public async Task<List<UploadedFileModel>> GetFilesByIssueIdAsync(int issueId)
        {
            return await _context.UploadedFiles
                .Where(f => f.IssueID == issueId)
                .ToListAsync();
        }
        //------------------------------------------------------------------------------------------------------------------------------------------//
    }
}
//--------------------------------------------------------X END OF FILE X-------------------------------------------------------------------//