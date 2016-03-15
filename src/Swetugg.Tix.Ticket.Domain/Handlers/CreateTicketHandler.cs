using System;
using CommonDomain.Persistence;
using Swetugg.Tix.Infrastructure;
using Swetugg.Tix.Ticket.Domain.Commands;

namespace Swetugg.Tix.Ticket.Domain.Handlers
{
    public class CreateTicketHandler : ICommandHandler<CreateTicket>
    {
        private readonly IRepository _repository;

        public CreateTicketHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(CreateTicket cmd)
        {
            var ticket = new Ticket(cmd.TicketId, cmd.TicketTypeId, cmd.CouponId);
            _repository.Save(ticket, Guid.NewGuid());
        }

    }
}