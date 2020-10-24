using Swetugg.Tix.Activity.Events;
using Swetugg.Tix.Activity.Views;
using Swetugg.Tix.Infrastructure;

namespace Swetugg.Tix.Activity.ViewBuilder
{

    public class ActivityOverviewEventApplier : EventApplierBase<ActivityOverview>
    {

        public ActivityOverviewEventApplier()
        {
            RegisterHandlers();
        }

        
        private ActivityOverview Handle(ActivityOverview view, ActivityCreated evt)
        {
            if (view == null)
            {
                view = new ActivityOverview();
            }
            view.ActivityId = evt.AggregateId;
            return view;
        }

        private ActivityOverview Handle(ActivityOverview view, TicketTypeAdded evt)
        {
            view.TicketTypes++;
            return view;
        }

        private ActivityOverview Handle(ActivityOverview view, TicketTypeRemoved evt)
        {
            view.TicketTypes--;
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
            view.FreeSeats--;
            return view;
        }

        private ActivityOverview Handle(ActivityOverview view, SeatReturned evt)
        {
            view.FreeSeats++;
            return view;
        }

    }
}
