using Microsoft.Azure.Cosmos.Table;
using Swetugg.Tix.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Swetugg.Tix.Order.Views.TableStorage
{
    public class OrderViewEntity : TableEntity, IViewEntity<OrderView>
    {
        public Guid OrderId { get; set; }
        public Guid? ActivityId { get; set; }
        public int Revision { get; set; }
        public string TicketsJson { get; set; }

        public void FromView(OrderView view)
        {
            OrderId = view.OrderId;
            PartitionKey = view.OrderId.ToString();
            RowKey = view.OrderId.ToString();
            Revision = view.Revision;

            TicketsJson = JsonSerializer.Serialize(view.Tickets, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }

        public OrderView ToView()
        {
            return new OrderView
            {
                OrderId = this.OrderId,
                ActivityId = this.ActivityId,
                Revision = this.Revision,
                Tickets = this.TicketsJson != null ? JsonSerializer.Deserialize<List<OrderTicket>>(this.TicketsJson, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }) : new List<OrderTicket>()
            };
        }
    }
}
