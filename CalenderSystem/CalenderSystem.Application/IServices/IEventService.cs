using CalenderSystem.Application.DTOs;
using CalenderSystem.Application.Feature.Events.Queries.Response;
using CalenderSystem.Domain.Entities;
using CalenderSystem.Domain.Entities.Identity;

namespace CalenderSystem.Application.IServices
{
    public interface IEventService
    {
        #region Google Calendar Events CRUD
        Task<PaginatedResponse<GetGoogleCalendarEventListResponse>> GetAllGoogleCalendarEvent(ApplicationUser user,
            string clientId, string clientSecret, DateTime? startDate, DateTime? endDate, string? searchQuery,
            int page, int pageSize);
        Task<string> AddGoogleCalendarEvent(AddEventDTO eventDto, ApplicationUser user,
            string clientId, string clientSecret);
        Task<string> UpdateGoogleCalendarEvent(UpdateEventDTO updatedEventDto,
            ApplicationUser user, string clientId, string clientSecret);
        Task<bool> DeleteGoogleCalendarEvent(string eventId,
            ApplicationUser user, string clientId, string clientSecret);
        #endregion
        #region My Database Events CRUD
        Task<IEnumerable<Event>> GetAllEventsAsync();
        Task<Event> GetEventByIdAsync(int id);

        Task<string> AddEventAsync(Event Dto);
        Task<Event> UpdateEventAsync(Event Dto);
        Task<string> DeleteEventAsync(int id);
        #endregion
    }
}
