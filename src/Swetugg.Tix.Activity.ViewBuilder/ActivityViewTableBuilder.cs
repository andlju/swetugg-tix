using Swetugg.Tix.Activity.Views;
using Swetugg.Tix.Activity.Views.TableStorage;
using Swetugg.Tix.Infrastructure.Views;

namespace Swetugg.Tix.Activity.ViewBuilder
{

    public class ActivityViewTableBuilder : TableStorageViewBuilder<ActivityOverview, ActivityViewEntity>
    {
        private const string _tableName = "activityview";

        public ActivityViewTableBuilder(string storageConnectionString) : base(storageConnectionString, _tableName, new ActivityOverviewEventApplier())
        {
        }

        protected override (string, string) GetKeys(string bucketId, string viewId)
        {
            return (bucketId, viewId);
        }
    }
}