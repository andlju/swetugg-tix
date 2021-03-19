using Microsoft.Azure.Cosmos.Table;
using Swetugg.Tix.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Swetugg.Tix.Activity.Views.TableStorage
{
    public class ActivityViewEntity : TableEntity, IViewEntity<ActivityOverview>
    {
        public ActivityViewEntity()
        {

        }

        public Guid ActivityId { get; set; }
        public int Revision { get; set; }
        public Guid CreatedByUserId { get; set; }
        public string Name { get; set; }
        public int TotalSeats { get; set; }
        public int FreeSeats { get; set; }
        public string TicketTypesJson { get; set; }

        public void FromView(ActivityOverview view)
        {
            var key = view.ActivityId.ToString();
            PartitionKey = key;
            RowKey = key;
            ActivityId = view.ActivityId;
            Revision = view.Revision;
            Name = view.Name;
            TotalSeats = view.TotalSeats;
            FreeSeats = view.FreeSeats;
            CreatedByUserId = view.CreatedByUserId;
            TicketTypesJson = JsonSerializer.Serialize(view.TicketTypes, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }

        public ActivityOverview ToView()
        {
            return new ActivityOverview
            {
                ActivityId = this.ActivityId,
                Revision = this.Revision,
                Name = this.Name,
                TotalSeats = this.TotalSeats,
                FreeSeats = this.FreeSeats,
                CreatedByUserId = this.CreatedByUserId,
                TicketTypes = this.TicketTypesJson != null ? JsonSerializer.Deserialize<List<TicketType>>(this.TicketTypesJson, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }) : new List<TicketType>()
            };
        }
    }
}