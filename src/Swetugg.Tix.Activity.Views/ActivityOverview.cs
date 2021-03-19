using Swetugg.Tix.Infrastructure;
using System;
using System.Collections.Generic;

namespace Swetugg.Tix.Activity.Views
{
    public class ActivityOverview : IView
    {
        public ActivityOverview()
        {
            TicketTypes = new List<TicketType>();
        }

        public Guid ActivityId { get; set; }
        public Guid CreatedByUserId { get; set; }
        public int Revision { get; set; }
        public string Name { get; set; }
        public int TotalSeats { get; set; }
        public int FreeSeats { get; set; }
        public List<TicketType> TicketTypes { get; set; }
    }

}
