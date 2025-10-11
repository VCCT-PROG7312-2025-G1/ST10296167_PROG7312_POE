using AnnouncementModel = ST10296167_PROG7312_POE.Models.Announcement;

namespace ST10296167_PROG7312_POE.Repository.Announcement
{
    public interface IAnnouncementRepository
    {
        Task<List<AnnouncementModel>> GetAllAnnouncementsAsync();
        Task<AnnouncementModel> GetByIdAsync(int id);
        Task<AnnouncementModel> AddAsync(AnnouncementModel announcement);
    }
}
//--------------------------------------------------------X END OF FILE X-------------------------------------------------------------------//