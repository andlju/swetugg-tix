using System;

namespace Swetugg.Tix.Activity.Domain.Commands
{
    public class ReserveSeat : ActivityCommand
    {
        public Guid TicketTypeId { get; set; }
        public Guid? CouponId { get; set; }

        public string Reference { get; set; }
    }
}