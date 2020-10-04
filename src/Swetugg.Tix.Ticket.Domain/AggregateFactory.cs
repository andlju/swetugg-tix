using NEventStore.Domain;
using NEventStore.Domain.Persistence;
using System;

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