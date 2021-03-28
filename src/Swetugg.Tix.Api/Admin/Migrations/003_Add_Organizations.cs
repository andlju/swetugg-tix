using FluentMigrator;

namespace Swetugg.Tix.Api.Admin.Migrations
{
    [Migration(3)]
    public class AddOrganizations : Migration
    {
        public override void Up()
        {

            Create.Table("Organization")
                .InSchema("Access")
                .WithColumn("OrganizationId").AsGuid().PrimaryKey()
                .WithColumn("Name").AsString(400)
                .WithColumn("LastUpdated").AsDateTime2().NotNullable();


            Create.Table("OrganizationUser")
                .InSchema("Access")
                .WithColumn("OrganizationId").AsGuid().NotNullable()
                .WithColumn("UserId").AsGuid().NotNullable()
                .WithColumn("LastUpdated").AsDateTime2().NotNullable();

            Create.PrimaryKey().OnTable("OrganizationUser")
                .WithSchema("Access")
                .Columns("OrganizationId", "UserId");

        }

        public override void Down()
        {
            Delete.Table("Organization").InSchema("Access");
            Delete.Table("OrganizationUser").InSchema("Access");
        }
    }
}
