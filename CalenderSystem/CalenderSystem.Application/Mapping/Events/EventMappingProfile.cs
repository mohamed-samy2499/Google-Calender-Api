using AutoMapper;
using CalenderSystem.Application.Feature.Events.Commands.Models;
using CalenderSystem.Application.Feature.Events.Queries.Response;
using CalenderSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalenderSystem.Application.Mapping.Events
{
    public class EventMappingProfile : Profile
    {
        public EventMappingProfile()
        {
            CreateMap<Event,GetEventResponse>();
            CreateMap<GetEventResponse,Event >();

            CreateMap<Event,GetEventListResponse>();
            CreateMap<AddEventCommand, Event>();
            CreateMap<UpdateEventCommand, Event>();
            CreateMap<GetEventResponse, UpdateEventCommand > ();


        }
    }
}
