﻿using Swetugg.Tix.Infrastructure;
using Swetugg.Tix.Order.Events;
using Swetugg.Tix.Order.Views;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Swetugg.Tix.Order.ViewBuilder
{


    public class OrderViewEventApplier: EventApplierBase<OrderView>
    {
        private OrderView Handle(OrderView view, OrderCreated evt)
        {
            view = new OrderView();
            view.Tickets = new List<OrderTicket>();
            view.OrderId = evt.AggregateId;
            view.ActivityId = evt.ActivityId;
            return view;
        }

        private OrderView Handle(OrderView view, TicketAdded evt)
        {
            view.Tickets.Add(new OrderTicket
            {
                ActivityId = view.ActivityId.GetValueOrDefault(),
                TicketId = evt.TicketId,
                OrderId = view.OrderId,
                TicketTypeId = evt.TicketTypeId
            });
            return view;
        }

        private OrderView Handle(OrderView view, TicketCancelled evt)
        {
            var ticket = GetTicket(view, evt.TicketId);
            ticket.Cancelled = true;
            return view;
        }

        private OrderView Handle(OrderView view, SeatReserved evt)
        {
            var ticket = GetTicket(view, evt.TicketId);
            ticket.TicketReference = evt.TicketReference;
            return view;
        }

        private OrderView Handle(OrderView view, SeatReturned evt)
        {
            var ticket = GetTicket(view, evt.TicketId);
            ticket.TicketReference = null;
            return view;
        }

        private static OrderTicket GetTicket(OrderView view, Guid ticketId)
        {
            return view.Tickets.FirstOrDefault(t => t.TicketId == ticketId);
        }

    }
}