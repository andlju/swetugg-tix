using System;

namespace Swetugg.Tix.Ticket.Domain.Commands
{
    public class CreateTicket : TicketCommand
    {
        public Guid TicketTypeId { get; set; }
        public Guid? CouponId { get; set; }
    }
}