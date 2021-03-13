using FluentMigrator;
using System;

namespace Swetugg.Tix.Api.Admin.Migrations
{
    [Migration(2)]
    public class AddUsers : Migration
    {
        public override void Up()
        {
            Create.Schema("Users");

            Create.Table("User")
                .InSchema("Users")
                .WithColumn("UserId").AsGuid().PrimaryKey()
                .WithColumn("Name").AsString(400)
                .WithColumn("Status").AsInt32()
                .WithColumn("LastUpdated").AsDateTime2().NotNullable();

            Create.Table("Issuer")
                .InSchema("Users")
                .WithColumn("IssuerId").AsGuid().PrimaryKey()
                .WithColumn("Name").AsString(400)
                .WithColumn("IssuerIdentifier").AsString(1000);

            Create.Table("UserLogin")
                .InSchema("Users")
                .WithColumn("IssuerId").AsGuid().NotNullable()
                .WithColumn("Subject").AsString(400).NotNullable()
                .WithColumn("UserId").AsGuid().NotNullable()
                .WithColumn("LastUpdated").AsDateTime2().NotNullable();


            Create.PrimaryKey().OnTable("UserLogin")
                .WithSchema("Users")
                .Columns("IssuerId", "Subject");

            Create.ForeignKey()
                .FromTable("UserLogin").InSchema("Users")
                .ForeignColumn("IssuerId")
                .ToTable("Issuer").InSchema("Users")
                .PrimaryColumn("IssuerId");

            Create.ForeignKey()
                .FromTable("UserLogin").InSchema("Users")
                .ForeignColumn("UserId")
                .ToTable("User").InSchema("Users")
                .PrimaryColumn("UserId");

            Insert
                .IntoTable("Issuer").InSchema("Users")
                .Row(new { IssuerId = Guid.NewGuid(), Name = "Swetugg Local", IssuerIdentifier = "https://swetuggtixlocal.b2clogin.com/866d2d59-fb99-4bbf-b901-4398de29c751/v2.0/" });
        }

        public override void Down()
        {
            Delete.Table("UserLogin").InSchema("Users");
            Delete.Table("Issuer").InSchema("Users");
            Delete.Table("User").InSchema("Users");
            Delete.Schema("Users");
        }
    }
}
