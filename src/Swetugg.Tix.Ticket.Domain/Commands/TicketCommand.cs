using System;

namespace Swetugg.Tix.Ticket.Domain.Commands
{
    public class TicketCommand : ITicketCommand
    {
        public Guid TicketId { get; set; }
        public Guid CommandId { get; set; }
    }
}