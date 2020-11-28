using Swetugg.Tix.Infrastructure;
using System;
using System.Collections.Generic;

namespace Swetugg.Tix.Order.Views
{

    public class OrderView : IView
    {
        public Guid OrderId { get; set; }
        public Guid? ActivityId { get; set; }
        public int Revision { get; set; }
        public List<OrderTicket> Tickets { get; set; }
    }

    public enum OrderTicketStatus
    {
        Pending,
        Confirmed,
        Denied,
        Cancelled
    }

    public class OrderTicket
    {
        public Guid TicketId { get; set; }
        public Guid OrderId { get; set; }
        public Guid ActivityId { get; set; }
        public Guid TicketTypeId { get; set; }
        public string TicketReference { get; set; }
        public OrderTicketStatus Status { get; set; }
        public string StatusCode => Status.ToString();
    }
}
