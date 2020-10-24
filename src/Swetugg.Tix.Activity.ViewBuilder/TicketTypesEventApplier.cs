using Swetugg.Tix.Activity.Events;
using Swetugg.Tix.Activity.Views;
using Swetugg.Tix.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Swetugg.Tix.Activity.ViewBuilder
{
    public class TicketTypesEventApplier : EventApplierBase<TicketTypesView>
    {
        public TicketTypesEventApplier()
        {
            RegisterHandlers();
        }

        private TicketTypesView Handle(TicketTypesView view, ActivityCreated evt)
        {
            if (view != null)
            {
                view = new TicketTypesView();
            }
            view.ActivityId = evt.AggregateId;
            view.TicketTypes = new List<TicketType>();
            return view;
        }

        private TicketTypesView Handle(TicketTypesView view, TicketTypeAdded evt)
        {
            view.TicketTypes.Add(new TicketType { TicketTypeId = evt.TicketTypeId });
            return view;
        }

        private TicketTypesView Handle(TicketTypesView view, TicketTypeRemoved evt)
        {
            view.TicketTypes.RemoveAll(tt => tt.TicketTypeId == evt.TicketTypeId);
            return view;
        }

        private TicketType GetTicketType(TicketTypesView view, Guid ticketTypeId)
        {
            return view.TicketTypes.FirstOrDefault(tt => tt.TicketTypeId == ticketTypeId);
        }

        private TicketTypesView Handle(TicketTypesView view, TicketTypeLimitIncreased evt)
        {
            var tt = GetTicketType(view, evt.TicketTypeId);
            tt.Limit += evt.Seats;
            return view;
        }

        private TicketTypesView Handle(TicketTypesView view, TicketTypeLimitDecreased evt)
        {
            var tt = GetTicketType(view, evt.TicketTypeId);
            tt.Limit -= evt.Seats;
            return view;
        }

        private TicketTypesView Handle(TicketTypesView view, TicketTypeLimitRemoved evt)
        {
            var tt = GetTicketType(view, evt.TicketTypeId);
            tt.Limit = null;
            return view;
        }

        private TicketTypesView Handle(TicketTypesView view, SeatReserved evt)
        {
            var tt = GetTicketType(view, evt.TicketTypeId);
            tt.Reserved++;
            return view;
        }

        private TicketTypesView Handle(TicketTypesView view, SeatReturned evt)
        {
            var tt = GetTicketType(view, evt.TicketTypeId);
            tt.Reserved--;
            return view;
        }


    }
}
