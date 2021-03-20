using NEventStore.Domain.Persistence;
using Swetugg.Tix.Infrastructure;
using Swetugg.Tix.Order.Commands;
using System;
using System.Threading.Tasks;

namespace Swetugg.Tix.Order.Domain.Handlers
{
    public class CreateOrderHandler : OrderCommandHandler<CreateOrder>
    {

        public CreateOrderHandler(IRepository repository, ICommandLog commandLog) : base(repository, commandLog)
        {
        }

        protected override void HandleCommand(Order order, CreateOrder cmd)
        {

        }

        protected override Order GetOrder(CreateOrder cmd)
        {
            return new Order(cmd.OrderId, cmd.ActivityId, cmd.ActivityOwnerId);
        }
    }
}