using System;
using System.Collections.Generic;

namespace Swetugg.Tix.Order.Commands
{
    public class CreateOrderWithTickets : OrderCommand
    {
        public class TicketOrder
        {
            public int Quantity { get; set; }
            public Guid TicketTypeId { get; set; }
        }
        public Guid ActivityId { get; set; }
        public List<TicketOrder> Tickets { get; set; }
    }

}