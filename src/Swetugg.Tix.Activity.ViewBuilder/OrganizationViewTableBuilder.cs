using Swetugg.Tix.Activity.Views;
using Swetugg.Tix.Activity.Views.TableStorage;
using Swetugg.Tix.Infrastructure.Views;

namespace Swetugg.Tix.Activity.ViewBuilder
{
    public class OrganizationViewTableBuilder : TableStorageViewBuilder<OrganizationOverview, OrganizationViewEntity>
    {
        private const string _tableName = "organizationview";

        public OrganizationViewTableBuilder(string storageConnectionString) : base(storageConnectionString, _tableName, new OrganizationOverviewEventApplier())
        {
        }

        protected override (string, string) GetKeys(string bucketId, string viewId)
        {
            return (bucketId, bucketId);
        }
    }

}