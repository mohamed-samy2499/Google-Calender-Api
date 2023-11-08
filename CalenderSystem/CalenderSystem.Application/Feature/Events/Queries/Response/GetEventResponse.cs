using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalenderSystem.Application.Feature.Events.Queries.Response
{
    public class GetEventResponse
    {
        public int Id { get; set; }
        public string GoogleCalendarEventId { get; set; }
        public string? Summary { get; set; }

        public string? Description { get; set; }

        public DateTime StartDateTime { get; set; }

        public DateTime EndDateTime { get; set; }
        public string? Location { get; set; }
    }
}
