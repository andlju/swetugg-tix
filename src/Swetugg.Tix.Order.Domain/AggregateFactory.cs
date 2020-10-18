using NEventStore.Domain;
using NEventStore.Domain.Persistence;
using System;

namespace Swetugg.Tix.Order.Domain
{
    public class AggregateFactory : IConstructAggregates
    {
        public IAggregate Build(Type type, Guid id, IMemento snapshot)
        {
            if (type == typeof(Order))
                return Order.Build();

            return (IAggregate)Activator.CreateInstance(type);
        }
    }
}