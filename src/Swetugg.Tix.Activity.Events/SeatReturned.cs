using System;

namespace Swetugg.Tix.Activity.Events
{
    public class SeatReturned : EventBase
    {
        public Guid TicketTypeId { get; set; }
        public Guid? CouponId { get; set; }
        public string Reference { get; set; }
    }
}