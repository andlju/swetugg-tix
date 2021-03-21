using Swetugg.Tix.Order.Commands;
using Swetugg.Tix.Tests.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Swetugg.Tix.Order.Domain.Tests
{
    public static class OrderCommandsExtensions
    {
        public static GivenOrderCommands Order(this IGivenCommands given, Guid orderId, Guid activityId, Guid activityOwnerId)
        {
            given.AddCommand(new CreateOrder()
            {
                OrderId = orderId,
                ActivityId = activityId,
                ActivityOwnerId = activityOwnerId
            });
            return new GivenOrderCommands(given, orderId);
        }

        public static GivenOrderCommands WithAddedTicket(this GivenOrderCommands given, Guid ticketTypeId)
        {
            given.AddCommand(new AddTickets() { Tickets = new List<AddTickets.TicketOrder>() { new AddTickets.TicketOrder { TicketTypeId = ticketTypeId, Quantity = 1 } } });
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