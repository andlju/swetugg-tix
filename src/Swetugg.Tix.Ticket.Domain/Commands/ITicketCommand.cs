using System;

namespace Swetugg.Tix.Ticket.Domain.Commands
{
    public interface ITicketCommand
    {
        Guid TicketId { get; }
        Guid CommandId { get; }
    }
}