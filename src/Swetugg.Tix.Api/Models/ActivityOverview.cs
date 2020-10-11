using System;

namespace Swetugg.Tix.Api.Models
{
    public class ActivityOverview
    {
        public Guid ActivityId { get; set; }
        public string Name { get; set; }
        public int FreeSeats { get; set; }
        public int TotalSeats { get; set; }
        public int TicketTypes { get; set; }
    }
}