using System;

namespace Swetugg.Tix.Api.Models
{
    public class TicketType
    {
        public Guid ActivityId { get; set; }
        public int Revision { get; set; }
        public Guid TicketTypeId { get; set; }
        public string Name { get; set; }
        public int? Limit { get; set; }
        public int Reserved { get; set; }
    }

    public class TicketTypesView
    {
        public int Revision { get; set; }
        public TicketType[] TicketTypes { get; set; }
    }
}
