using NEventStore.Domain.Core;
using System;
using ActivityCommands = Swetugg.Tix.Activity.Commands;
using ActivityEvents = Swetugg.Tix.Activity.Events;
using OrderCommands = Swetugg.Tix.Order.Commands;
using OrderEvents = Swetugg.Tix.Order.Events;

namespace Swetugg.Tix.Process
{
    public class OrderConfirmationSaga : SagaBase<object>
    {
        private Guid? _activityId;

        public OrderConfirmationSaga(string id)
        {
            Id = id;
            Register<OrderEvents.OrderCreated>(Handle);
            Register<OrderEvents.TicketAdded>(Handle);
            Register<OrderEvents.TicketCancelled>(Handle);
            Register<ActivityEvents.SeatReserved>(Handle);
            Register<ActivityEvents.SeatReservationFailed>(Handle);
            Register<ActivityEvents.SeatReturned>(Handle);
        }

        void Handle(OrderEvents.OrderCreated evt)
        {
            _activityId = evt.ActivityId;
        }

        void Handle(OrderEvents.TicketAdded evt)
        {
            // Try to reserve a seat for this ticket
            Dispatch(new ActivityCommands.ReserveSeat()
            {
                CommandId = Guid.NewGuid(),

                ActivityId = _activityId.Value,
                TicketTypeId = evt.TicketTypeId,
                OrderReference = Id.ToString()
            });
        }

        void Handle(OrderEvents.TicketCancelled evt)
        {
            // Return the seat
        }

        void Handle(ActivityEvents.SeatReserved evt)
        {
            // A seat has been reserved for this order. Let's confirm it.
            Dispatch(new OrderCommands.ConfirmReservedSeat()
            {
                CommandId = Guid.NewGuid(),

                OrderId = Guid.Parse(Id),
                TicketTypeId = evt.TicketTypeId,
                TicketReference = evt.TicketReference
            });
        }

        void Handle(ActivityEvents.SeatReservationFailed evt)
        {
            // A seat has been reserved for this order. Let's confirm it.
            Dispatch(new OrderCommands.DenyReservedSeat()
            {
                CommandId = Guid.NewGuid(),

                OrderId = Guid.Parse(Id),
                TicketTypeId = evt.TicketTypeId,
                ReasonCode = evt.ReasonCode
            });
        }

        void Handle(ActivityEvents.SeatReturned evt)
        {
            // A seat has been returned for this order. Let's let the order know
            Dispatch(new OrderCommands.ConfirmReturnedSeat()
            {
                CommandId = Guid.NewGuid(),

                OrderId = Guid.Parse(Id),
                TicketTypeId = evt.TicketTypeId,
                TicketReference = evt.TicketReference
            });
        }

    }
}
