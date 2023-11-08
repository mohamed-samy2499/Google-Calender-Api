using CalenderSystem.Domain.Entities;
using CalenderSystem.Application.IServices;
using CalenderSystem.Infrastructure.Repositories.EventRepositories;
using CalenderSystem.Application.DTOs;
using Microsoft.AspNetCore.Identity;
using System.Net.Http;
using System.Text;
using CalenderSystem.Domain.Entities.Identity;
using System.Text.Json;
using Google.Apis.Auth.OAuth2;
using System.Threading;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;
using Google.Apis.Calendar.v3.Data;
using System.Linq.Expressions;

namespace CalenderSystem.Application.Services
{
    public class EventService : IEventService
    {
        #region CTOR
        private readonly IEventRepository _eventRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly HttpClient _httpClient;
        public EventService(IEventRepository eventRepository, UserManager<ApplicationUser> userManager)
        {
			_eventRepository = eventRepository;
            _userManager = userManager;
            _httpClient = new HttpClient(); 
        }
        #endregion
        #region Google Calendar Events
        //Google Calendar Events CRUD
        public async Task<string> AddGoogleCalendarEvent(AddEventDTO eventDto, ApplicationUser user,
            string clientId,string clientSecret)
        {
            try
            {

                //get user credentials
                var token = new TokenResponse
                {
                    RefreshToken = user.GoogleRefreshToken
                };
                var credentials = new UserCredential(new GoogleAuthorizationCodeFlow(
                new GoogleAuthorizationCodeFlow.Initializer
                {
                    ClientSecrets = new ClientSecrets
                    {
                        ClientId = clientId,
                        ClientSecret = clientSecret
                    }

                }), "user", token);
                //initiate the google calendar service
                var service = new CalendarService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credentials,
                });

                //construct the event

                Google.Apis.Calendar.v3.Data.Event newEvent = new Google.Apis.Calendar.v3.Data.Event()
                {
                    Summary = eventDto.Summary,
                    Location = eventDto.Location,
                    Description = eventDto.Description,
                    Start = new EventDateTime()
                    {
                        DateTime = eventDto.StartTime,
                        TimeZone = "Africa/Cairo",
                    },
                    End = new EventDateTime()
                    {
                        DateTime = eventDto.EndTime,
                        TimeZone = "Africa/Cairo",
                    },
                    Reminders = new Google.Apis.Calendar.v3.Data.Event.RemindersData()
                    {
                        UseDefault = false,
                        Overrides = new EventReminder[] {

                            new EventReminder() {Method = "email", Minutes = 30},

                            new EventReminder() {Method = "popup", Minutes = 15},

                            new EventReminder() {Method = "popup", Minutes = 1},
                        }
                    }
                };
                //execute the event insertion into the primary calendar
                EventsResource.InsertRequest insertRequest = service
                    .Events.Insert(newEvent, "primary");
                Google.Apis.Calendar.v3.Data.Event createdEvent = insertRequest.Execute();
                return createdEvent.Id;
            }
            catch(Exception e) 
            {
                return string.Empty;
            }
            
        }

        public async Task<string> UpdateGoogleCalendarEvent(UpdateEventDTO updatedEventDto, 
            ApplicationUser user, string clientId, string clientSecret)
        {
            try
            {
                // Get user credentials
                var token = new TokenResponse
                {
                    RefreshToken = user.GoogleRefreshToken
                };
                var credentials = new UserCredential(new GoogleAuthorizationCodeFlow(
                    new GoogleAuthorizationCodeFlow.Initializer
                    {
                        ClientSecrets = new ClientSecrets
                        {
                            ClientId = clientId,
                            ClientSecret = clientSecret
                        }
                    }), "user", token);

                // Initiate the Google Calendar service
                var service = new CalendarService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credentials,
                });

                // Retrieve the existing event by event ID
                EventsResource.GetRequest getRequest = service.Events.Get("primary", updatedEventDto.GoogleCalendarEventId);
                Google.Apis.Calendar.v3.Data.Event existingEvent = getRequest.Execute();

                if (existingEvent == null)
                {
                    return string.Empty; // Event not found
                }

                // Update the event properties
                existingEvent.Summary = updatedEventDto.Summary;
                existingEvent.Location = updatedEventDto.Location;
                existingEvent.Description = updatedEventDto.Description;
                existingEvent.Start.DateTime = updatedEventDto.StartTime;
                existingEvent.End.DateTime = updatedEventDto.EndTime;

                // Execute the event update
                EventsResource.UpdateRequest updateRequest = service.Events.Update(existingEvent,
                    "primary", updatedEventDto.GoogleCalendarEventId);
                Google.Apis.Calendar.v3.Data.Event updatedEvent = updateRequest.Execute();

                return updatedEvent.Id;
            }
            catch (Exception e)
            {
                return string.Empty;
            }
        }
        public async Task<bool> DeleteGoogleCalendarEvent(string eventId, 
            ApplicationUser user, string clientId, string clientSecret)
        {
            try
            {
                // Get user credentials
                var token = new TokenResponse
                {
                    RefreshToken = user.GoogleRefreshToken
                };
                var credentials = new UserCredential(new GoogleAuthorizationCodeFlow(
                    new GoogleAuthorizationCodeFlow.Initializer
                    {
                        ClientSecrets = new ClientSecrets
                        {
                            ClientId = clientId,
                            ClientSecret = clientSecret
                        }
                    }), "user", token);

                // Initialize the Google Calendar service
                var service = new CalendarService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credentials,
                });

                // Execute the event deletion by eventId
                await service.Events.Delete("primary", eventId).ExecuteAsync();

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        #endregion
        #region My Database Events


        //Events in Database CRUD
        public async Task<string> AddEventAsync(CalenderSystem.Domain.Entities.Event Dto)
        {
            return await _eventRepository.CreateAsync(Dto);
        }

        public async Task<string> DeleteEventAsync(int id)
        {
            return await _eventRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<CalenderSystem.Domain.Entities.Event>> GetAllEventsAsync()
        {
            var includes = new Expression<Func<CalenderSystem.Domain.Entities.Event, object>>[] {
                myEvent => myEvent.User
            };
            return await _eventRepository.GetAllAsync(includes);
        }

        public async Task<CalenderSystem.Domain.Entities.Event> GetEventByIdAsync(int id)
        {
            return await _eventRepository.GetByIdAsync(id);
        }

        public async Task<CalenderSystem.Domain.Entities.Event> UpdateEventAsync(CalenderSystem.Domain.Entities.Event Dto)
        {
            return await _eventRepository.UpdateAsync(Dto);
        }

        #endregion
    }
}
