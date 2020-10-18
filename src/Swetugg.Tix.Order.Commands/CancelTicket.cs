using System;

namespace Swetugg.Tix.Order.Commands
{
    public class CancelTicket : OrderCommand
    {
        public Guid TicketTypeId { get; set; }
    }

}