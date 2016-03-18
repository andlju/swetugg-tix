using System;

namespace Swetugg.Tix.Activity.Commands
{
    public class ReturnSeat : ActivityCommand
    {
        public Guid TicketTypeId { get; set; }
        public Guid? CouponId { get; set; }

        public string Reference { get; set; }
    }
}