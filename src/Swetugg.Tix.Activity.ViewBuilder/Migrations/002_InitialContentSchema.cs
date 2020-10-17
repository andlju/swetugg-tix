using FluentMigrator;

namespace Swetugg.Tix.Activity.ViewBuilder.Migrations
{
    [Migration(2)]
    public class InitialContentSchema : Migration
    {
        public override void Up()
        {
            Create.Schema("ActivityContent");

            Create.Table("Activity")
                .InSchema("ActivityContent")
                .WithColumn("ActivityId").AsGuid().PrimaryKey()
                .WithColumn("Name").AsString(400)
                .WithColumn("LastUpdated").AsDateTime2().NotNullable();


            Create.Table("TicketType")
                .InSchema("ActivityContent")
                .WithColumn("ActivityId").AsGuid().NotNullable().NotNullable()
                .WithColumn("TicketTypeId").AsGuid().NotNullable()
                .WithColumn("Name").AsString(400)
                .WithColumn("LastUpdated").AsDateTime2().NotNullable();

            Create.PrimaryKey().OnTable("TicketType")
                .WithSchema("ActivityContent")
                .Columns("ActivityId", "TicketTypeId");

        }

        public override void Down()
        {
            Delete.Table("Activity").InSchema("ActivityContent");
            Delete.Table("TicketType").InSchema("ActivityContent");
            Delete.Schema("ActivityContent");
        }
    }
}
