using CalenderSystem.Application.Bases;

using CalenderSystem.Application.Feature.Events.Queries.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalenderSystem.Application.Feature.Events.Commands.Models
{
    public class UpdateEventCommand : IRequest<Response<GetEventResponse>>
    {
        public int Id { get; set; }
        public string? GoogleCalendarEventId { get; set; }
        public string? UserId { get; set; }
        public string? Summary { get; set; }

        public string? Description { get; set; }

        public DateTime StartDateTime { get; set; }

        public DateTime EndDateTime { get; set; }
        public string? Location { get; set; }
    }
}
