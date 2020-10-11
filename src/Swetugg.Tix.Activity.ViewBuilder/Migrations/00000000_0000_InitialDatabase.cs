using FluentMigrator;

namespace Swetugg.Tix.Activity.ViewBuilder.Migrations
{
    [Migration(0)]
    public class InitialDatabase : Migration
    {
        public override void Up()
        {
            Create.Schema("ActivityViews");

            Create.Table("ActivityOverview")
                .InSchema("ActivityViews")
                .WithColumn("ActivityId").AsGuid().PrimaryKey()
                .WithColumn("TicketTypes").AsInt32()
                .WithColumn("TotalSeats").AsInt32()
                .WithColumn("FreeSeats").AsInt32();


            Create.Table("TicketType")
                .InSchema("ActivityViews")
                .WithColumn("ActivityId").AsGuid().NotNullable().NotNullable()
                .WithColumn("TicketTypeId").AsGuid().NotNullable()
                .WithColumn("Limit").AsInt32().Nullable()
                .WithColumn("Reserved").AsInt32().WithDefaultValue(0).NotNullable();

            Create.PrimaryKey().OnTable("TicketType")
                .WithSchema("ActivityViews")
                .Columns("ActivityId", "TicketTypeId");

        }

        public override void Down()
        {
            Delete.Table("ActivityOverview").InSchema("ActivityViews");
            Delete.Table("TicketType").InSchema("ActivityViews");
            Delete.Schema("ActivityViews");
        }
    }
}
