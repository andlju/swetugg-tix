using System;

namespace Swetugg.Tix.Order.Commands
{
    public class AddTicket : OrderCommand
    {
        public Guid TicketTypeId { get; set; }
    }

}