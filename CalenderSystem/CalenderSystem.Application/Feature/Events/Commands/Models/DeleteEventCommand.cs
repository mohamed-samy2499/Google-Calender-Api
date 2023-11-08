using CalenderSystem.Application.Bases;

using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalenderSystem.Application.Feature.Events.Commands.Models
{
    public class DeleteEventCommand :IRequest<Response<string>>
    {
        public int EventId { get; set; }
    }
}
