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
    public class GetEventListQuery : IRequest<Response<IEnumerable<GetEventListResponse>>>
    {
    }
}
