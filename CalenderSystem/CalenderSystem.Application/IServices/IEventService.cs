using CalenderSystem.Domain.Entities;

namespace CalenderSystem.Application.IServices
{
    public interface IEventService
    {
        Task<IEnumerable<Event>> GetAllEventsAsync();
        Task<Event> GetEventByIdAsync(int id);

        Task<string> AddEventAsync(Event Dto);
        Task<Event> UpdateEventAsync(Event Dto);
        Task<string> DeleteEventAsync(int id);
    }
}
