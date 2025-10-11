using AnnouncementModel = ST10296167_PROG7312_POE.Models.Announcement;

namespace ST10296167_PROG7312_POE.Services.Announcement
{
    public interface IAnnouncementService
    {
        Task<List<AnnouncementModel>> GetRecentAnnouncementsAsync(int count);
        Task<bool> AddAnnouncementAsync(AnnouncementModel announcement);
    }
}
//--------------------------------------------------------X END OF FILE X-------------------------------------------------------------------//