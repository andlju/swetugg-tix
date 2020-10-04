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
                .WithColumn("ActivityId").AsGuid().Nullable()
                .WithColumn("JsonBody").AsCustom("ntext").Nullable()
                .WithColumn("Status").AsString(100).NotNullable()
                .WithColumn("LastUpdated").AsDateTime2().NotNullable();

            Create.Table("CommandLogMessages")
                .InSchema("ActivityLogs")
                .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                .WithColumn("CommandId").AsGuid().ForeignKey("FK_CommandLogErrors_CommandLog", "ActivityLogs", "CommandLog", "CommandId")
                .WithColumn("Severity").AsInt32()
                .WithColumn("Code").AsString(100)
                .WithColumn("Message").AsCustom("ntext")
                .WithColumn("Timestamp").AsDateTime2().NotNullable();
        }

        public override void Down()
        {
            Delete.Table("CommandLog").InSchema("ActivityLogs");
            Delete.Table("CommandLogMessages").InSchema("ActivityLogs");
        }
    }
}
