using System;
using CommonDomain;
using CommonDomain.Persistence;

namespace Swetugg.Tix.Ticket.Domain
{
    public class AggregateFactory : IConstructAggregates
    {
        public IAggregate Build(Type type, Guid id, IMemento snapshot)
        {
            if (type == typeof(Ticket))
                return Ticket.Build();

            return (IAggregate)Activator.CreateInstance(type);
        }
    }
}