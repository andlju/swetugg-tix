using System;

namespace Swetugg.Tix.Activity.Views
{
    public class ActivityOverview
    {
        public Guid ActivityId { get; set; }
        public string Name { get; set; }
        public int TotalSeats { get; set; }
        public int FreeSeats { get; set; }
    }
}
