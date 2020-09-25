using FluentMigrator;

namespace Swetugg.Tix.Activity.ViewBuilder.Migrations
{
  [Migration(20200925154300)]
  public class AddCheckpointTable : Migration
  {
    public override void Up()
    {
      Create.Table("Checkpoint")
          .WithColumn("Name").AsString(200).PrimaryKey()
          .WithColumn("LastCheckpoint").AsInt64();
    }

    public override void Down()
    {
      Delete.Table("Checkpoints");
    }
  }
}
