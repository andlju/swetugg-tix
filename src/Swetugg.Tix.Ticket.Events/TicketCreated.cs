using System;

namespace Swetugg.Tix.Ticket.Events
{
    public class TicketCreated : EventBase
    {
        public Guid TicketTypeId { get; set; }
        public Guid? CouponId { get; set; }
    }

}