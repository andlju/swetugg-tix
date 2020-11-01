using Swetugg.Tix.Activity.Events;
using Swetugg.Tix.Activity.Views;
using Swetugg.Tix.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Swetugg.Tix.Activity.ViewBuilder
{

    public class ActivityOverviewEventApplier : EventApplierBase<ActivityOverview>
    {
        
        private ActivityOverview Handle(ActivityOverview view, ActivityCreated evt)
        {
            view = new ActivityOverview();
            view.ActivityId = evt.AggregateId;
            view.TicketTypes = new List<TicketType>();
            return view;
        }

        private ActivityOverview Handle(ActivityOverview view, TicketTypeAdded evt)
        {
            view.TicketTypes.Add(new TicketType { ActivityId = evt.AggregateId, TicketTypeId = evt.TicketTypeId });
            return view;
        }

        private ActivityOverview Handle(ActivityOverview view, TicketTypeRemoved evt)
        {
            view.TicketTypes.RemoveAll(tt => tt.TicketTypeId == evt.TicketTypeId);
            return view;
        }

        private TicketType GetTicketType(ActivityOverview view, Guid ticketTypeId)
        {
            return view.TicketTypes.FirstOrDefault(tt => tt.TicketTypeId == ticketTypeId);
        }

        private ActivityOverview Handle(ActivityOverview view, TicketTypeLimitIncreased evt)
        {
            var tt = GetTicketType(view, evt.TicketTypeId);
            tt.Limit = tt.Limit.GetValueOrDefault(0) + evt.Seats;
            return view;
        }

        private ActivityOverview Handle(ActivityOverview view, TicketTypeLimitDecreased evt)
        {
            var tt = GetTicketType(view, evt.TicketTypeId);
            tt.Limit = tt.Limit.GetValueOrDefault(0) - evt.Seats;
            return view;
        }

        private ActivityOverview Handle(ActivityOverview view, TicketTypeLimitRemoved evt)
        {
            var tt = GetTicketType(view, evt.TicketTypeId);
            tt.Limit = null;
            return view;
        }

        private ActivityOverview Handle(ActivityOverview view, SeatsAdded evt)
        {
            view.FreeSeats += evt.Seats;
            view.TotalSeats += evt.Seats;
            return view;
        }

        private ActivityOverview Handle(ActivityOverview view, SeatsRemoved evt)
        {
            view.FreeSeats -= evt.Seats;
            view.TotalSeats -= evt.Seats;
            return view;
        }

        private ActivityOverview Handle(ActivityOverview view, SeatReserved evt)
        {
            var tt = GetTicketType(view, evt.TicketTypeId);
            tt.Reserved++;
            view.FreeSeats--;
            return view;
        }

        private ActivityOverview Handle(ActivityOverview view, SeatReturned evt)
        {
            var tt = GetTicketType(view, evt.TicketTypeId);
            tt.Reserved--;
            view.FreeSeats++;
            return view;
        }

    }
}
