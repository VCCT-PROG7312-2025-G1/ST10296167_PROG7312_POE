using Microsoft.EntityFrameworkCore;
using ST10296167_PROG7312_POE.Data;
using ST10296167_PROG7312_POE.Models;
using FeedbackModel = ST10296167_PROG7312_POE.Models.Feedback;

namespace ST10296167_PROG7312_POE.Repository.Feedback
{
    public class FeedbackRepository : IFeedbackRepository
    {
        private readonly ApplicationDbContext _context;

        // Constructor
        //------------------------------------------------------------------------------------------------------------------------------------------//
        public FeedbackRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------//

        // Methods
        //------------------------------------------------------------------------------------------------------------------------------------------//
        // Add a new feedback to the database
        public async Task<FeedbackModel> AddFeedbackAsync(FeedbackModel feedback)
        {
            _context.Feedbacks.Add(feedback);
            await _context.SaveChangesAsync();
            return feedback;
        }

        // Get all feedback from the database
        public async Task<List<FeedbackModel>> GetAllFeedbackAsync()
        {
            return await _context.Feedbacks
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();
        }
        //------------------------------------------------------------------------------------------------------------------------------------------//
    }
}
//--------------------------------------------------------X END OF FILE X-------------------------------------------------------------------//