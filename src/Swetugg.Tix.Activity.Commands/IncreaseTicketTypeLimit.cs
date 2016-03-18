using System;

namespace Swetugg.Tix.Activity.Commands
{
    public class IncreaseTicketTypeLimit : ActivityCommand
    {
        public Guid TicketTypeId { get; set; }
        public int Seats { get; set; }
    }
}