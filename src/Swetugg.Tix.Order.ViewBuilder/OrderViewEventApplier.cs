using Swetugg.Tix.Infrastructure;
using Swetugg.Tix.Order.Events;
using Swetugg.Tix.Order.Views;
using System;

namespace Swetugg.Tix.Order.ViewBuilder
{


    public class OrderViewEventApplier: EventApplierBase<OrderView>
    {
        private OrderView Handle(OrderView view, OrderCreated evt)
        {
            if(view == null)
            {
                view = new OrderView();
            }
            view.OrderId = evt.AggregateId;
            return view;
        }
    }
}
