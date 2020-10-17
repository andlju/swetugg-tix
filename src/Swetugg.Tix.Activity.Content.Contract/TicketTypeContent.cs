using System;

namespace Swetugg.Tix.Activity.Content.Contract
{
    public class TicketTypeContent
    {
        public Guid ActivityId { get; set; }
        public Guid TicketTypeId { get; set; }
        public string Name { get; set; }
    }

}
