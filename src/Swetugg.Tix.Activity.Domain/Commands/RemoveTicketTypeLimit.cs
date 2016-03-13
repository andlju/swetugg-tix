using System;

namespace Swetugg.Tix.Activity.Domain.Commands
{
    public class RemoveTicketTypeLimit : ActivityCommand
    {
        public Guid TicketTypeId { get; set; }
    }
}