using AnnouncementModel = ST10296167_PROG7312_POE.Models.Announcement;
using ST10296167_PROG7312_POE.Data;
using ST10296167_PROG7312_POE.Repository.Announcement;

namespace ST10296167_PROG7312_POE.Services.Announcement
{
    public class AnnouncementService : IAnnouncementService
    {
        private readonly DataStore _dataStore;
        private readonly IAnnouncementRepository _announcementRepository;
        private readonly ApplicationDbContext _context;

        // Constructor
        //------------------------------------------------------------------------------------------------------------------------------------------//
        public AnnouncementService(ApplicationDbContext context, IAnnouncementRepository announcementRepository, DataStore dataStore)
        {
            _context = context;
            _announcementRepository = announcementRepository;
            _dataStore = dataStore;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------//

        // Methods
        //------------------------------------------------------------------------------------------------------------------------------------------//
        public async Task<List<AnnouncementModel>> GetRecentAnnouncementsAsync(int count)
        {
            throw new NotImplementedException();
        }
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
            //------------------------------------------------------------------------------------------------------------------------------------------//
        }
    }
}
//--------------------------------------------------------X END OF FILE X-------------------------------------------------------------------//