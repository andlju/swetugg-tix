using System;

namespace Swetugg.Tix.Ticket.Commands
{
    public interface ITicketCommand
    {
        Guid TicketId { get; }
        Guid CommandId { get; }
    }
}