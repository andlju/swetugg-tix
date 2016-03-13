using System;

namespace Swetugg.Tix.Activity.Domain.Commands
{
    public class AddSeats : ActivityCommand
    {
        public int Seats { get; set; }
    }
}