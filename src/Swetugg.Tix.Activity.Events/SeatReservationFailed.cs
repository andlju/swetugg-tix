using System;

namespace Swetugg.Tix.Activity.Events
{
    public class SeatReservationFailed : EventBase
    {
        public Guid TicketTypeId { get; set; }
        public string OrderReference { get; set; }
        public string ReasonCode { get; set; }
    }
}