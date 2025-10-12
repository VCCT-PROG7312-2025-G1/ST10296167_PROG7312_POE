using EventModel = ST10296167_PROG7312_POE.Models.Event;

namespace ST10296167_PROG7312_POE.Services.Event
{
    public interface IEventService
    {
        Task<List<EventModel>> GetAllEventsAsync();

        Task<bool> AddEventAsync(EventModel eventModel);

      //  Task<List<EventModel>> SearchEventsAsync(string? category, DateTime? startDate, DateTime? endDate);

        Task<List<EventModel>> GetRecommendedEventsAsync();

        Task<List<string>> GetAllCategoriesAsync();

        Task<List<EventModel>> SearchEventsAsync(string? category, DateTime? startDate, DateTime? endDate);
    }
}
//--------------------------------------------------------X END OF FILE X-------------------------------------------------------------------//