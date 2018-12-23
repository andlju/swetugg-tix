using System;
using NEventStore.Domain;
using NEventStore.Domain.Persistence;

namespace Swetugg.Tix.Activity.Domain
{
    public class AggregateFactory : IConstructAggregates
    {
        public IAggregate Build(Type type, Guid id, IMemento snapshot)
        {
            if (type == typeof(Activity))
                return Activity.Build();

            return (IAggregate)Activator.CreateInstance(type);
        }
    }
}