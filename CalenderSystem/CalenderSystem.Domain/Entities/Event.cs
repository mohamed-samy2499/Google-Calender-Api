using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalenderSystem.Domain.Entities.Identity;

namespace CalenderSystem.Domain.Entities
{
    public class Event: BaseEntity
    {
        public string? Summary { get; set; } = null!;
        public string? Description { get; set; } = null!;
        public string? Location { get; set; } = null!;

        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public string? RefreshToken { get; set; }

        // ---------- relations -----------
        public string? GoogleCalendarEventId { get; set; } = null!;
        public string? UserId { get; set; } = null!;
        public ApplicationUser? User { get; set; }
    }

}
