using FluentMigrator;

namespace Swetugg.Tix.Activity.ViewBuilder.Migrations
{
    [Migration(20200927115200)]
    public class AddTicketTypeTable : Migration
    {
        public override void Up()
        {
            Create.Table("TicketType")
                .WithColumn("ActivityId").AsGuid().NotNullable().NotNullable()
                .WithColumn("TicketTypeId").AsGuid().NotNullable()
                .WithColumn("Name").AsString(200).NotNullable()
                .WithColumn("Limit").AsInt32().Nullable()
                .WithColumn("Reserved").AsInt32().WithDefaultValue(0).NotNullable();

            Create.PrimaryKey().OnTable("TicketType").Columns("ActivityId", "TicketTypeId");
        }

        public override void Down()
        {
            Delete.Table("TicketType");
        }
    }
}