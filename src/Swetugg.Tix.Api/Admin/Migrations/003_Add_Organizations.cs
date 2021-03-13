using FluentMigrator;

namespace Swetugg.Tix.Api.Admin.Migrations
{
    [Migration(3)]
    public class AddOrganizations : Migration
    {
        public override void Up()
        {
            Create.Schema("Orgs");

            Create.Table("Organization")
                .InSchema("Orgs")
                .WithColumn("OrganizationId").AsGuid().PrimaryKey()
                .WithColumn("Name").AsString(400)
                .WithColumn("LastUpdated").AsDateTime2().NotNullable();


            Create.Table("OrganizationUser")
                .InSchema("Orgs")
                .WithColumn("OrganizationId").AsGuid().NotNullable()
                .WithColumn("UserId").AsGuid().NotNullable()
                .WithColumn("LastUpdated").AsDateTime2().NotNullable();

            Create.PrimaryKey().OnTable("OrganizationUser")
                .WithSchema("Orgs")
                .Columns("OrganizationId", "UserId");

        }

        public override void Down()
        {
            Delete.Table("Organization").InSchema("Orgs");
            Delete.Table("OrganizationUser").InSchema("Orgs");
            Delete.Schema("Orgs");
        }
    }
}
