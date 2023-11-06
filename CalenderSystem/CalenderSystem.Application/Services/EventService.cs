using CalenderSystem.Domain.Entities;
using CalenderSystem.Application.IServices;
using CalenderSystem.Infrastructure.Repositories.EventRepositories;

namespace CalenderSystem.Application.Services
{
    public class EventService : IEventService
    {
        #region CTOR
        private readonly IEventRepository _eventRepository;
        public EventService(IEventRepository eventRepository)
        {
			_eventRepository = eventRepository;
        }
        #endregion

        public async Task<string> AddEventAsync(Event Dto)
        {
            return await _eventRepository.CreateAsync(Dto);
        }

        public async Task<string> DeleteEventAsync(int id)
        {
            return await _eventRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<Event>> GetAllEventsAsync()
        {
            return await _eventRepository.GetAllAsync();
        }

        public async Task<Event> GetEventByIdAsync(int id)
        {
            return await _eventRepository.GetByIdAsync(id);
        }

        public async Task<Event> UpdateEventAsync(Event Dto)
        {
            return await _eventRepository.UpdateAsync(Dto);
        }
    }
}
