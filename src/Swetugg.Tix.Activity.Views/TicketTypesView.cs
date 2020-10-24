using Swetugg.Tix.Infrastructure;
using System;
using System.Collections.Generic;

namespace Swetugg.Tix.Activity.Views
{
    public class TicketTypesView : IView
    {
        public Guid ActivityId { get; set; }

        public int Revision { get; set; }
        public List<TicketType> TicketTypes { get; set; }
    }

}
