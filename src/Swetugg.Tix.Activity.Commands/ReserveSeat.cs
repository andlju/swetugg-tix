using System;

namespace Swetugg.Tix.Activity.Commands
{
    public class ReserveSeat : ActivityCommand
    {
        public Guid TicketTypeId { get; set; }

        public string OrderReference { get; set; }
    }
}