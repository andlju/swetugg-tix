using Swetugg.Tix.Activity.Views;
using System;

namespace Swetugg.Tix.Api.Activities
{
    public class TicketTypesView
    {
        public int Revision { get; set; }
        public TicketType[] TicketTypes { get; set; }
    }
}
