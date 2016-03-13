using System;

namespace Swetugg.Tix.Activity.Domain.Commands
{
    public class RemoveSeats : ActivityCommand
    {
        public int Seats { get; set; }
    }
}