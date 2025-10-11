using ST10296167_PROG7312_POE.Data;
using ST10296167_PROG7312_POE.Repository.Event;
using EventModel = ST10296167_PROG7312_POE.Models.Event;

namespace ST10296167_PROG7312_POE.Services.Event
{
    public class EventService: IEventService
    {
        private readonly DataStore _dataStore;
        private readonly IEventRepository _eventRepository;
        private readonly ApplicationDbContext _context;

        // Constructor
        //------------------------------------------------------------------------------------------------------------------------------------------//
        public EventService(ApplicationDbContext context, IEventRepository eventRepository, DataStore dataStore)
        {
            _context = context;
            _eventRepository = eventRepository;
            _dataStore = dataStore;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------//

        // Methods
        //------------------------------------------------------------------------------------------------------------------------------------------//
        public async Task<List<EventModel>> GetAllEventsAsync()
        {
            throw new NotImplementedException();
        }
        public async Task<bool> AddEventAsync(EventModel newEvent)
        {
            try
            {
                // Add to DB
                var savedEvent = await _eventRepository.AddEventAsync(newEvent);
                newEvent.ID = savedEvent.ID;

                var eventDateTime = newEvent.Date.ToDateTime(newEvent.Time);

                if(!_dataStore.EventsByDate.ContainsKey(eventDateTime))
                {
                    _dataStore.EventsByDate[eventDateTime] = new List<EventModel>();
                }

                // Add to sorted dictionary
                _dataStore.EventsByDate[eventDateTime].Add(newEvent);
                Console.WriteLine($"Event added to in-memory datastore");

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