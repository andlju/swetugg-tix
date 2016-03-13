using System;
using CommonDomain;
using CommonDomain.Persistence;

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