using AnnouncementModel = ST10296167_PROG7312_POE.Models.Announcement;
using ST10296167_PROG7312_POE.Data;
using ST10296167_PROG7312_POE.Repository.Announcement;

namespace ST10296167_PROG7312_POE.Services.Announcement
{
    public class AnnouncementService : IAnnouncementService
    {
        private readonly DataStore _dataStore;
        private readonly IAnnouncementRepository _announcementRepository;

        // Constructor
        //------------------------------------------------------------------------------------------------------------------------------------------//
        public AnnouncementService(IAnnouncementRepository announcementRepository, DataStore dataStore)
        {
            _announcementRepository = announcementRepository;
            _dataStore = dataStore;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------//

        // Methods
        //------------------------------------------------------------------------------------------------------------------------------------------//
        // Get recent announcements from the recent announcements stack
        public Task<List<AnnouncementModel>> GetRecentAnnouncementsAsync(int count)
        {
            var recentAnnouncements = _dataStore.RecentAnnouncements
                .Take(count)
                .ToList();

            return Task.FromResult(recentAnnouncements);
        }

        // Get all announcements from the recent announcements stack
        public Task<List<AnnouncementModel>> GetAllAnnouncementsAsync()
        {
            var allAnnouncements = _dataStore.RecentAnnouncements.ToList();
            return Task.FromResult(allAnnouncements);
        }

        // Add a new announcement to the database and the recent announcements stack
        public async Task<bool> AddAnnouncementAsync(AnnouncementModel announcement)
        {
            try
            {
                // Add to DB
                var savedAnnouncement = await _announcementRepository.AddAsync(announcement);
                announcement.ID = savedAnnouncement.ID;

                // Add to stack
                _dataStore.RecentAnnouncements.Push(announcement);
                Console.WriteLine($"Announcement added to in-memory datastore");

                return true;
            }
            catch
            {
                return false;
            }
        }
        //------------------------------------------------------------------------------------------------------------------------------------------//
    }
}
//--------------------------------------------------------X END OF FILE X-------------------------------------------------------------------//