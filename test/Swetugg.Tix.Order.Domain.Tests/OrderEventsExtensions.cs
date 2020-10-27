using NEventStore;
using Swetugg.Tix.Order.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Swetugg.Tix.Order.Domain.Tests
{
    public static class OrderEventsExtensions
    {
        public static Guid GetFirstTicketId(this IEnumerable<ICommit> commits, Guid ticketTypeId)
        {
            return commits.GetEvents<TicketAdded>().First(t => t.TicketTypeId == ticketTypeId).TicketId;
        }

        public static IEnumerable<TEvent> GetEvents<TEvent>(this IEnumerable<ICommit> commits)
        {
            return commits.SelectMany(c => c.Events).Select(e => e.Body).OfType<TEvent>();
        }
    }
}