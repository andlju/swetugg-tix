using Microsoft.Azure.Cosmos.Table;
using Swetugg.Tix.Infrastructure;
using System;

namespace Swetugg.Tix.Activity.Views.TableStorage
{
    public class OrganizationViewEntity : TableEntity, IViewEntity<OrganizationOverview>
    {
        public Guid OrganizationId { get; set; }
        public int Revision { get; set; }

        public int Activities { get; set; }

        public void FromView(OrganizationOverview view)
        {
            OrganizationId = view.OrganizationId;
            PartitionKey = view.OrganizationId.ToString();
            RowKey = view.OrganizationId.ToString();
            Activities = view.Activities;
        }

        public OrganizationOverview ToView()
        {
            return new OrganizationOverview
            {
                Activities = Activities,
                OrganizationId = OrganizationId,
                Revision = Revision
            };
        }
    }
}