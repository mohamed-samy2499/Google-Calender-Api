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
using Microsoft.Extensions.Logging;
using AutoMapper;
using CalenderSystem.Application.Feature.Events.Queries.Response;
using Microsoft.AspNetCore.Components;

namespace CalenderSystem.Application.Services
{
    public class EventService : IEventService
    {
        #region CTOR
        private readonly IEventRepository _eventRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly HttpClient _httpClient;
        private readonly IMapper _mapper;
        public EventService(IEventRepository eventRepository, UserManager<ApplicationUser> userManager
            ,IMapper mapper)
        {
			_eventRepository = eventRepository;
            _userManager = userManager;
            _mapper = mapper;
            _httpClient = new HttpClient(); 
        }
        #endregion
        #region Google Calendar Events
        //Google Calendar Events CRUD
        public async Task<List<GetEventListResponse>> GetAllGoogleCalendarEvent(ApplicationUser user,
            string clientId, string clientSecret, DateTime? startDate, DateTime? endDate, string? searchQuery)
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
                Google.Apis.Calendar.v3.Data.Events calendarEvents = service.Events.List("primary").Execute();


                if (calendarEvents.Items != null && calendarEvents.Items.Count > 0)
                {
                    var filteredEvents = calendarEvents.Items;

                    if (startDate.HasValue)
                    {
                        filteredEvents = filteredEvents.Where(e => e.Start.DateTime >= startDate).ToList();
                    }

                    if (endDate.HasValue)
                    {
                        filteredEvents = filteredEvents.Where(e => e.Start.DateTime <= endDate).ToList();
                    }

                    if (!string.IsNullOrEmpty(searchQuery))
                    {
                        filteredEvents = filteredEvents.Where(e => e.Summary.Contains(searchQuery)
                        || e.Description.Contains(searchQuery)).ToList();
                    }
                    var mappedResult = new List<GetEventListResponse>();
                    foreach(var eventItem in filteredEvents)
                    {
                        if(eventItem.Start.DateTime == null ||eventItem.End.DateTime == null)
                        {
                            DateTime start;
                            DateTime.TryParse(eventItem.Start.Date, out start);
                            DateTime end;
                            DateTime.TryParse(eventItem.End.Date, out end);
                            var mappedEvent = new GetEventListResponse()
                            {
                                Summary = eventItem.Summary,
                                Description = eventItem.Description.Replace("\\n","\n"),
                                StartDateTime = start,
                                EndDateTime = end,
                                Location = eventItem.Location
                            };
                            mappedResult.Add(mappedEvent);

                        }
                        else
                        {

                            var mappedEvent = new GetEventListResponse() 
                            {
                                Summary = eventItem.Summary,
                                Description = eventItem.Description,
                                StartDateTime = (DateTime)eventItem.Start.DateTime,
                                EndDateTime = (DateTime)eventItem.End.DateTime,
                                Location = eventItem.Location
                            };
                            mappedResult.Add(mappedEvent);
                        }
                    }

                    return mappedResult;
                }
                else
                {
                    return null;
                }

            }
            catch (Exception e)
            {
                return null;
            }
        }
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
            //var includes = new Expression<Func<CalenderSystem.Domain.Entities.Event, object>>[] {
            //    myEvent => myEvent.User
            //};
            return await _eventRepository.GetAllAsync();
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
