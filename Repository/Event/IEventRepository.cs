using EventModel = ST10296167_PROG7312_POE.Models.Event;

namespace ST10296167_PROG7312_POE.Repository.Event
{
    public interface IEventRepository
    {
        Task<List<EventModel>> GetAllEventsAsync();
        Task<EventModel> GetByIdAsync(int id);
        Task<EventModel> AddEventAsync(EventModel eventModel);
    }
}
//--------------------------------------------------------X END OF FILE X-------------------------------------------------------------------//