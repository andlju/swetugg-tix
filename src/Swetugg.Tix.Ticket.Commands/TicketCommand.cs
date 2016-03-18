using System;

namespace Swetugg.Tix.Ticket.Commands
{
    public class TicketCommand : ITicketCommand
    {
        public Guid TicketId { get; set; }
        public Guid CommandId { get; set; }
    }
}