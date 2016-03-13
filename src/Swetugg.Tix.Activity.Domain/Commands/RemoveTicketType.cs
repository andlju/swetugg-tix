using System;

namespace Swetugg.Tix.Activity.Domain.Commands
{
    public class RemoveTicketType : ActivityCommand
    {
        public Guid TicketTypeId { get; set; }
    }
}