using Microsoft.EntityFrameworkCore;
using ST10296167_PROG7312_POE.Data;
using AnnouncementModel = ST10296167_PROG7312_POE.Models.Announcement;

namespace ST10296167_PROG7312_POE.Repository.Announcement
{
    public class AnnouncementRepository: IAnnouncementRepository
    {
        private readonly ApplicationDbContext _context;

        // Constructor
        //------------------------------------------------------------------------------------------------------------------------------------------//
        public AnnouncementRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------//

        // Methods
        //------------------------------------------------------------------------------------------------------------------------------------------//
        // Add announcement to the database and return the added announcement
        public async Task<AnnouncementModel> AddAsync(AnnouncementModel announcement)
        {
            _context.Announcements.Add(announcement);
            await _context.SaveChangesAsync();
            Console.WriteLine($"Announcement added to db");
            return announcement;

        }

        // Get all announcements ordered by creation date
        public async Task<List<AnnouncementModel>> GetAllAnnouncementsAsync()
        {
            return await _context.Announcements
                .OrderBy(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<AnnouncementModel> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
        //------------------------------------------------------------------------------------------------------------------------------------------//
    }
}
//--------------------------------------------------------X END OF FILE X-------------------------------------------------------------------//