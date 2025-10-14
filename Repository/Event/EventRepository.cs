using Microsoft.EntityFrameworkCore;
using ST10296167_PROG7312_POE.Data;
using EventModel = ST10296167_PROG7312_POE.Models.Event;

namespace ST10296167_PROG7312_POE.Repository.Event
{
    public class EventRepository: IEventRepository
    {
        private readonly ApplicationDbContext _context;

        // Constructor
        //------------------------------------------------------------------------------------------------------------------------------------------//
        public EventRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------//

        // Methods
        //------------------------------------------------------------------------------------------------------------------------------------------//
        // Add event to the database and return the added event
        public async Task<EventModel> AddEventAsync(EventModel newEvent)
        {
            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync();
            Console.WriteLine($"Event added to db");
            return newEvent;
        }

        // Get and return all events in a list
        public async Task<List<EventModel>> GetAllEventsAsync()
        {
            return await _context.Events
                .ToListAsync();
        }

        public async Task<EventModel> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
        //------------------------------------------------------------------------------------------------------------------------------------------//
    }
}
//--------------------------------------------------------X END OF FILE X-------------------------------------------------------------------//