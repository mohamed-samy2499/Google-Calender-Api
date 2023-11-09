using CalenderSystem.Application.DTOs;
using CalenderSystem.Application.Feature.Events.Commands.Models;
using CalenderSystem.Application.Feature.Events.Commands.Validators;
using CalenderSystem.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using FluentValidation;
using FluentValidation.Results;

namespace CalenderSystem.Api.Helper
{
    public static class EventHelper
    {

        public static async Task<ValidationResult> ValidateAddEventAsync(AddEventDTO eventDto, ApplicationUser user)
        {
            var addEventValidationCheck = CreateAddEventCommand(eventDto, user, null);
            return await new AddEventValidator().ValidateAsync(addEventValidationCheck);
        }

        public  static async Task<ValidationResult> ValidateUpdateEventAsync(UpdateEventDTO eventDto, ApplicationUser user)
        {
            var updateEventValidationCheck = CreateUpdateEventCommand(eventDto, user);
            return await new UpdateEventValidator().ValidateAsync(updateEventValidationCheck);
        }

        public static AddEventCommand CreateAddEventCommand(AddEventDTO eventDto, ApplicationUser user, string googleCalendarEventId)
        {
            return new AddEventCommand()
            {
                Summary = eventDto.Summary,
                Description = eventDto.Description,
                Location = eventDto.Location,
                StartDateTime = eventDto.StartTime,
                EndDateTime = eventDto.EndTime,
                RefreshToken = user.GoogleRefreshToken,
                GoogleCalendarEventId = googleCalendarEventId,
                UserId = user.Id
            };
        }

        public static UpdateEventCommand CreateUpdateEventCommand(UpdateEventDTO eventDto, ApplicationUser user)
        {
            return new UpdateEventCommand()
            {
                Id = eventDto.Id,
                Summary = eventDto.Summary,
                Description = eventDto.Description,
                Location = eventDto.Location,
                StartDateTime = eventDto.StartTime,
                EndDateTime = eventDto.EndTime,
                GoogleCalendarEventId = eventDto.GoogleCalendarEventId,
                UserId = user.Id
            };
        }
    }
}
