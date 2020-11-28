using System;

namespace Swetugg.Tix.Order.Commands
{
    public class DenyReservedSeat : OrderCommand
    {
        public Guid TicketTypeId { get; set; }
        public string ReasonCode { get; set; }
    }

}