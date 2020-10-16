using FluentMigrator;

namespace Swetugg.Tix.Activity.ViewBuilder.Migrations
{
    [Migration(202010040947)]
    public class AddCommandLogTable : Migration
    {
        public override void Up()
        {
            Create.Schema("ActivityLogs");

            Create.Table("CommandLog")
                .InSchema("ActivityLogs")
                .WithColumn("CommandId").AsGuid().PrimaryKey()
                .WithColumn("AggregateId").AsString(200).Nullable()
                .WithColumn("CommandType").AsString(300).Nullable()
                .WithColumn("JsonBody").AsCustom("ntext").Nullable()
                .WithColumn("Status").AsString(100).NotNullable()
                .WithColumn("LastUpdated").AsDateTime2().NotNullable();

            Create.Table("CommandLogMessage")
                .InSchema("ActivityLogs")
                .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                .WithColumn("CommandId").AsGuid().ForeignKey("FK_CommandLogError_CommandLog", "ActivityLogs", "CommandLog", "CommandId")
                .WithColumn("Severity").AsInt32()
                .WithColumn("Code").AsString(100)
                .WithColumn("Message").AsCustom("ntext")
                .WithColumn("Timestamp").AsDateTime2().NotNullable();
        }

        public override void Down()
        {
            Delete.Table("CommandLog").InSchema("ActivityLogs");
            Delete.Table("CommandLogMessage").InSchema("ActivityLogs");
        }
    }
}
