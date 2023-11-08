using AutoMapper;
using CalenderSystem.Application.Bases;
using CalenderSystem.Application.Feature.Events.Queries.Models;
using CalenderSystem.Application.Feature.Events.Queries.Response;
using CalenderSystem.Application.IServices;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalenderSystem.Application.Feature.Events.Queries.Handlers
{
    public class EventsCommandHandler : ResponseHandler ,
                                 IRequestHandler<GetEventListQuery,Response<IEnumerable<GetEventListResponse>>>,
                                 IRequestHandler<GetEventByIdQuery, Response<GetEventResponse>>
    {
        #region Ctor
        private readonly IEventService _eventService;
        private IMapper _mapper;
        public EventsCommandHandler(IEventService eventService, IMapper mapper)
        {
            _eventService = eventService;
            _mapper = mapper;
        }
        #endregion
        #region CRUD
        public async Task<Response<IEnumerable<GetEventListResponse>>> Handle(GetEventListQuery request, CancellationToken cancellationToken)
        {
            var entitiesList = await _eventService.GetAllEventsAsync();
            if (entitiesList == null || entitiesList.Count() == 0)
            {
                return NotFound<IEnumerable<GetEventListResponse>>("there is no data");
            }
            else
            {
                var ListMapped = _mapper.Map<IEnumerable<GetEventListResponse>>(entitiesList);
                var result = Success(ListMapped);
                result.Meta = new { count = ListMapped.Count() };
                return result;
            }

        }
        public async Task<Response<GetEventResponse>> Handle(GetEventByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _eventService.GetEventByIdAsync(request.EventId);

            if (entity == null)
            {
                return NotFound<GetEventResponse>("Sorry, There is no data to display!");
            }
            else
            {
                var entityMapped = _mapper.Map<GetEventResponse>(entity);


                return Success(entityMapped);
            }
        }
        #endregion

    }
}
