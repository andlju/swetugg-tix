using FluentMigrator;

namespace Swetugg.Tix.Activity.ViewBuilder.Migrations
{
    [Migration(3)]
    public class AddTicketCommandLogTable : Migration
    {
        public override void Up()
        {
            Create.Schema("TicketLogs");

            Create.Table("CommandLog")
                .InSchema("TicketLogs")
                .WithColumn("CommandId").AsGuid().PrimaryKey()
                .WithColumn("AggregateId").AsString(200).Nullable()
                .WithColumn("Revision").AsInt32().Nullable()
                .WithColumn("CommandType").AsString(300).Nullable()
                .WithColumn("JsonBody").AsCustom("ntext").Nullable()
                .WithColumn("Status").AsString(100).NotNullable()
                .WithColumn("LastUpdated").AsDateTime2().NotNullable();

            Create.Table("CommandLogMessage")
                .InSchema("TicketLogs")
                .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                .WithColumn("CommandId").AsGuid().ForeignKey("FK_CommandLogError_CommandLog", "TicketLogs", "CommandLog", "CommandId")
                .WithColumn("Severity").AsInt32()
                .WithColumn("Code").AsString(100)
                .WithColumn("Message").AsCustom("ntext")
                .WithColumn("Timestamp").AsDateTime2().NotNullable();
        }

        public override void Down()
        {
            Delete.Table("CommandLog").InSchema("TicketLogs");
            Delete.Table("CommandLogMessage").InSchema("TicketLogs");
        }
    }
}
