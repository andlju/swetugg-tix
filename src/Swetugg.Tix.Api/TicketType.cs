using System;

namespace Swetugg.Tix.Api
{
    public class TicketType
    {
        public Guid ActivityId { get; set; }
        public Guid TicketTypeId { get; set; }
        public string Name { get; set; }
        public int? Limit { get; set; }
        public int Reserved { get; set; }
    }
}
