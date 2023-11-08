using AutoMapper;
using CalenderSystem.Application.Bases;
using CalenderSystem.Application.Feature.Events.Commands.Models;
using CalenderSystem.Application.Feature.Events.Queries.Response;
using CalenderSystem.Application.IServices;
using CalenderSystem.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalenderSystem.Application.Feature.Events.Commands.Handlers
{
    public class EventsCommandHandler : ResponseHandler, IRequestHandler<AddEventCommand, Response<GetEventResponse>>,
                                       IRequestHandler<DeleteEventCommand, Response<string>>,
                                       IRequestHandler<UpdateEventCommand, Response<GetEventResponse>>
    {
        #region Ctor
        private readonly IEventService _eventService;
        private readonly IMapper _mapper;
        public EventsCommandHandler(IEventService eventService, IMapper mapper)
        {
            _eventService = eventService;
            _mapper = mapper;
        }
        #endregion
        #region CRUD
        public async Task<Response<GetEventResponse>> Handle(AddEventCommand request, CancellationToken cancellationToken)
        {
            var myEvent = _mapper.Map<Event>(request);
            var result = await _eventService.AddEventAsync(myEvent);
            var resEvent = _mapper.Map<GetEventResponse>(myEvent);
            if (result != "success")
            { return BadRequest<GetEventResponse>(""); }
            else { return Created<GetEventResponse>(resEvent); }
        }

        public async Task<Response<GetEventResponse>> Handle(UpdateEventCommand request, CancellationToken cancellationToken)
        {
            var myEvent = _mapper.Map<Event>(request);
            var result = await _eventService.UpdateEventAsync(myEvent);
            var resEvent = _mapper.Map<GetEventResponse>(myEvent);
            if (result == null) { return BadRequest<GetEventResponse>(""); }
            else { return Success<GetEventResponse>(resEvent); }
        }

        public async Task<Response<string>> Handle(DeleteEventCommand request, CancellationToken cancellationToken)
        {
            var result = await _eventService.DeleteEventAsync(request.EventId);
            if (result != "success") { return BadRequest<string>(""); }
            else { return Deleted<string>(result); }
        }
        #endregion

    }
}
