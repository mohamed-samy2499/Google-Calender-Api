using CalenderSystem.Application.Bases;
using CalenderSystem.Application.Feature.Events.Queries.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalenderSystem.Application.Feature.Events.Queries.Models
{
    public class GetEventByIdQuery : IRequest<Response<GetEventResponse>>
    {
        public int EventId { get; set; }
    }
}
