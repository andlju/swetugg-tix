using FluentMigrator;

namespace Swetugg.Tix.Api.Admin.Migrations
{
    [Migration(1)]
    public class InitialDatabase : Migration
    {
        public override void Up()
        {
            Create.Schema("ActivityViews");

            Create.Table("ActivityOverview")
                .InSchema("ActivityViews")
                .WithColumn("ActivityId").AsGuid().PrimaryKey()
                .WithColumn("Revision").AsInt32().NotNullable()
                .WithColumn("TicketTypes").AsInt32()
                .WithColumn("TotalSeats").AsInt32()
                .WithColumn("FreeSeats").AsInt32();


            Create.Table("TicketType")
                .InSchema("ActivityViews")
                .WithColumn("ActivityId").AsGuid().NotNullable()
                .WithColumn("TicketTypeId").AsGuid().NotNullable()
                .WithColumn("Revision").AsInt32().NotNullable()
                .WithColumn("Limit").AsInt32().Nullable()
                .WithColumn("Reserved").AsInt32().WithDefaultValue(0).NotNullable();

            Create.PrimaryKey().OnTable("TicketType")
                .WithSchema("ActivityViews")
                .Columns("ActivityId", "TicketTypeId");

            Create.Schema("OrderViews");

            Create.Table("OrderView")
                .InSchema("OrderViews")
                .WithColumn("OrderId").AsGuid().PrimaryKey()
                .WithColumn("Revision").AsInt32().NotNullable()
                .WithColumn("ActivityId").AsGuid().Nullable();

            Create.Table("OrderTicket")
                .InSchema("OrderViews")
                .WithColumn("OrderId").AsGuid().PrimaryKey()
                .WithColumn("ActivityId").AsGuid()
                .WithColumn("TicketTypeId").AsGuid()
                .WithColumn("TicketReference").AsString(100);

            Create.ForeignKey()
                .FromTable("OrderTicket").InSchema("OrderViews")
                .ForeignColumn("OrderId")
                .ToTable("OrderView").InSchema("OrderViews")
                .PrimaryColumn("OrderId");
        }

        public override void Down()
        {
            Delete.Table("ActivityOverview").InSchema("ActivityViews");
            Delete.Table("TicketType").InSchema("ActivityViews");
            Delete.Schema("ActivityViews");
            Delete.Table("OrderTicket").InSchema("OrderViews");
            Delete.Table("OrderView").InSchema("OrderViews");
            Delete.Schema("OrderViews");
        }
    }
}
