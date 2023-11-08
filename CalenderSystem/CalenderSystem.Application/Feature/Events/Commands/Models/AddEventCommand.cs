using CalenderSystem.Application.Bases;
using CalenderSystem.Application.Feature.Events.Queries.Response;
using CalenderSystem.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalenderSystem.Application.Feature.Events.Commands.Models
{
    public class AddEventCommand: IRequest<Response<GetEventResponse>>
    {
        public string? Summary { get; set; }

        public string? Description { get; set; }

        public DateTime StartDateTime { get; set; }

        public DateTime EndDateTime { get; set; }
        public string? Location { get; set; }
        public string? RefreshToken { get; set; }


        ////////////////////////////////
        public string? UserId { get; set; }
        public string? GoogleCalendarEventId { get; set; }

    }
}
