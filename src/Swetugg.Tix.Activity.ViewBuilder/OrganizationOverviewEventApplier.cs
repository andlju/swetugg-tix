using Swetugg.Tix.Activity.Events;
using Swetugg.Tix.Activity.Views;
using Swetugg.Tix.Infrastructure;

namespace Swetugg.Tix.Activity.ViewBuilder
{
    public class OrganizationOverviewEventApplier : EventApplierBase<OrganizationOverview>
    {

        private OrganizationOverview Handle(OrganizationOverview view, ActivityCreated evt)
        {
            if (view == null)
                view = new OrganizationOverview();
            view.OrganizationId = evt.OwnerId;
            view.Activities += 1;

            return view;
        }
    }
}
