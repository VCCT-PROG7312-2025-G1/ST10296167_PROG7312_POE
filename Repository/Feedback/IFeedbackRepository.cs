using ST10296167_PROG7312_POE.Models;
using FeedbackModel = ST10296167_PROG7312_POE.Models.Feedback;

namespace ST10296167_PROG7312_POE.Repository.Feedback
{
    public interface IFeedbackRepository
    {
        Task<FeedbackModel> AddFeedbackAsync(FeedbackModel feedback);
        Task<List<FeedbackModel>> GetAllFeedbackAsync();
    }
}
//--------------------------------------------------------X END OF FILE X-------------------------------------------------------------------//