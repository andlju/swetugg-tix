using NEventStore.Domain.Persistence;
using Swetugg.Tix.Infrastructure;
using Swetugg.Tix.Ticket.Commands;
using System;
using System.Threading.Tasks;

namespace Swetugg.Tix.Ticket.Domain.Handlers
{
    public class CreateTicketHandler : TicketCommandHandler<CreateTicket>
    {

        public CreateTicketHandler(IRepository repository, ICommandLog commandLog) : base(repository, commandLog)
        {
        }

        protected override void HandleCommand(Ticket activity, CreateTicket cmd)
        {

        }

        protected override Ticket GetTicket(CreateTicket cmd)
        {
            return new Ticket(cmd.TicketId, cmd.ActivityId, cmd.TicketTypeId, cmd.CouponId);
        }
    }
}