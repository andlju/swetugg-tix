using System;

namespace Swetugg.Tix.Activity.Commands
{
    public class RemoveTicketType : ActivityCommand
    {
        public Guid TicketTypeId { get; set; }
    }
}