using EventModel = ST10296167_PROG7312_POE.Models.Event;

namespace ST10296167_PROG7312_POE.Services.Event
{
    public interface IEventService
    {
        Task<List<EventModel>> GetAllEventsAsync();

        Task<bool> AddEventAsync(EventModel eventModel);
    }
}
//--------------------------------------------------------X END OF FILE X-------------------------------------------------------------------//