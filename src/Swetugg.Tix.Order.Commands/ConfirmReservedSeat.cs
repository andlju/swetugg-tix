using System;

namespace Swetugg.Tix.Order.Commands
{
    public class ConfirmReservedSeat : OrderCommand
    {
        public Guid TicketTypeId { get; set; }
        public string TicketReference { get; set; }
    }

}