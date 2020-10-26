using Swetugg.Tix.Infrastructure;
using System;

namespace Swetugg.Tix.Order.Views
{
    public class OrderView : IView
    {
        public Guid OrderId { get; set; }
        public int Revision { get; set; }
    }
}
