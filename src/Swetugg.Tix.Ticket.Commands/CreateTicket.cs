using System;

namespace Swetugg.Tix.Ticket.Commands
{
    public class CreateTicket : TicketCommand
    {
        public Guid ActivityId { get; set; }
        public Guid TicketTypeId { get; set; }
        public Guid? CouponId { get; set; }
    }
}