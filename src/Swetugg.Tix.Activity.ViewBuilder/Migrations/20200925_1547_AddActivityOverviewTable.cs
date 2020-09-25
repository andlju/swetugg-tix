using FluentMigrator;

namespace Swetugg.Tix.Activity.ViewBuilder.Migrations
{
  [Migration(20200925154700)]
  public class AddActivityOverviewTable : Migration
  {
    public override void Up()
    {
      Create.Table("ActivityOverview")
          .WithColumn("ActivityId").AsGuid().PrimaryKey()
          .WithColumn("Name").AsString()
          .WithColumn("TotalSeats").AsInt32()
          .WithColumn("FreeSeats").AsInt32();
    }

    public override void Down()
    {
      Delete.Table("ActivityOverview");
    }
  }
}
