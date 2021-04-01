using Swetugg.Tix.Infrastructure;
using System;

namespace Swetugg.Tix.Activity.Views
{
    public class OrganizationOverview : IView
    {
        public Guid OrganizationId { get; set; }
        public int Revision { get; set; }
        public int Activities { get; set; }
    }

}
