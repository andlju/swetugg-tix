using System;

namespace Swetugg.Tix.Activity.Commands
{
    public class CreateActivity : ActivityCommand
    {
        public string Name { get; set; }
        public Guid OwnerId { get; set; }
    }
}