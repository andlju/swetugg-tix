using System;

namespace Swetugg.Tix.Activity.Views
{

    public class TicketType
    {
        public Guid TicketTypeId { get; set; }
        public int Revision { get; set; }
        public string Name { get; set; }
        public int? Limit { get; set; }
        public int Reserved { get; set; }
    }

}
