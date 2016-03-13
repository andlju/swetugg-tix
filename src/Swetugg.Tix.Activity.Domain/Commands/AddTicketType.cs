using System;

namespace Swetugg.Tix.Activity.Domain.Commands
{
    public class AddTicketType : ActivityCommand
    {
        public Guid TicketTypeId { get; set; }
    }
}