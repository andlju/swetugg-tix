using FluentMigrator;

namespace Swetugg.Tix.Activity.ViewBuilder.Migrations
{
    [Migration(20200926114300)]
    public class AddTicketTypesToActivityOverviewTable : Migration
    {
        public override void Up()
        {
            Alter.Table("ActivityOverview").
                AddColumn("TicketTypes").AsInt32().WithDefaultValue(0).NotNullable();
        }

        public override void Down()
        {
            Delete.Column("TicketTypes").FromTable("ActivityOverview");
        }
    }
}