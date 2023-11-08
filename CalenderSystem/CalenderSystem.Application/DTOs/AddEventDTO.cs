using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalenderSystem.Application.DTOs
{
    public class AddEventDTO
    {
        public string? Summary {  get; set; }

        public string? Description { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime {  get; set; }
        public string? Location { get; set; }

    }
}
