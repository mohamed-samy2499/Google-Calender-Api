using CalenderSystem.Application.DTOs;
using CalenderSystem.Domain.Entities;
using CalenderSystem.Domain.Entities.Identity;

namespace CalenderSystem.Application.IServices
{
    public interface IEventService
    {
        Task<string> AddGoogleCalendarEvent(AddEventDTO eventDto, ApplicationUser user,
            string clientId, string clientSecret);
        Task<IEnumerable<Event>> GetAllEventsAsync();
        Task<Event> GetEventByIdAsync(int id);

        Task<string> AddEventAsync(Event Dto);
        Task<Event> UpdateEventAsync(Event Dto);
        Task<string> DeleteEventAsync(int id);
    }
}
