using Swetugg.Tix.Infrastructure;
using System;

namespace Swetugg.Tix.Activity.Views
{
    public class ActivityOverview : IView
    {
        public Guid ActivityId { get; set; }
        public int Revision { get; set; }
        public string Name { get; set; }
        public int TotalSeats { get; set; }
        public int FreeSeats { get; set; }
        public int TicketTypes { get; set; }
    }

}
