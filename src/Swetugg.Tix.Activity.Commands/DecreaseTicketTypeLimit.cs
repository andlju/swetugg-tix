﻿using System;

namespace Swetugg.Tix.Activity.Commands
{
    public class DecreaseTicketTypeLimit : ActivityCommand
    {
        public Guid TicketTypeId { get; set; }
        public int Seats { get; set; }
    }
}