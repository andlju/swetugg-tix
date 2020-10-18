using System;

namespace Swetugg.Tix.Activity.Commands
{
    public class ReturnSeat : ActivityCommand
    {
        public Guid TicketTypeId { get; set; }

        public string OrderReference { get; set; }
        public string TicketReference { get; set; }
    }
}