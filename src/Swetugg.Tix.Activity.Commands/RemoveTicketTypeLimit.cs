using System;

namespace Swetugg.Tix.Activity.Commands
{
    public class RemoveTicketTypeLimit : ActivityCommand
    {
        public Guid TicketTypeId { get; set; }
    }
}