using System;

namespace Swetugg.Tix.Activity.Commands
{
    public class AddTicketType : ActivityCommand
    {
        public Guid TicketTypeId { get; set; }
        public string Name { get; set; }
    }
}