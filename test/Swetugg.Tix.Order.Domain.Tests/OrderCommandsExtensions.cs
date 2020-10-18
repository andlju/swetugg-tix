using Swetugg.Tix.Order.Commands;
using Swetugg.Tix.Tests.Helpers;
using System;

namespace Swetugg.Tix.Order.Domain.Tests
{
    public static class OrderCommandsExtensions
    {
        public static GivenOrderCommands Order(this IGivenCommands given, Guid orderId, Guid activityId)
        {
            given.AddCommand(new CreateOrder()
            {
                OrderId = orderId,
                ActivityId = activityId,
            });
            return new GivenOrderCommands(given, activityId);
        }

        public static GivenOrderCommands WithAddedTicket(this GivenOrderCommands given, Guid ticketTypeId)
        {
            given.AddCommand(new AddTicket() { TicketTypeId = ticketTypeId });
            return given;
        }

        public static GivenOrderCommands WithReservedSeat(this GivenOrderCommands given, Guid ticketTypeId, string ticketReference)
        {
            given.AddCommand(new ConfirmReservedSeat() { TicketTypeId = ticketTypeId, TicketReference = ticketReference });
            return given;
        }

        public static GivenOrderCommands WithReturnedSeat(this GivenOrderCommands given, Guid ticketTypeId, string ticketReference)
        {
            given.AddCommand(new ConfirmReturnedSeat() { TicketTypeId = ticketTypeId, TicketReference = ticketReference });
            return given;
        }

        public static GivenOrderCommands WithCancelledTicket(this GivenOrderCommands given, Guid ticketTypeId)
        {
            given.AddCommand(new CancelTicket() { TicketTypeId = ticketTypeId });
            return given;
        }

    }
}