using FluentMigrator;
using Microsoft.Extensions.Options;
using Swetugg.Tix.Api.Options;
using System;

namespace Swetugg.Tix.Api.Admin.Migrations
{
    [Migration(2)]
    public class AddUsers : Migration
    {
        private readonly string _issuerIdentifier;

        public AddUsers(ApiOptions options)
        {
            _issuerIdentifier = options.IssuerIdentifier;
        }

        public override void Up()
        {
            Create.Schema("Access");

            Create.Table("User")
                .InSchema("Access")
                .WithColumn("UserId").AsGuid().PrimaryKey()
                .WithColumn("Name").AsString(400)
                .WithColumn("Status").AsInt32()
                .WithColumn("LastUpdated").AsDateTime2().NotNullable();

            Create.Table("Issuer")
                .InSchema("Access")
                .WithColumn("IssuerId").AsGuid().PrimaryKey()
                .WithColumn("Name").AsString(400)
                .WithColumn("IssuerIdentifier").AsString(1000);

            Create.Table("UserLogin")
                .InSchema("Access")
                .WithColumn("IssuerId").AsGuid().NotNullable()
                .WithColumn("Subject").AsString(400).NotNullable()
                .WithColumn("UserId").AsGuid().NotNullable()
                .WithColumn("LastUpdated").AsDateTime2().NotNullable();


            Create.PrimaryKey().OnTable("UserLogin")
                .WithSchema("Access")
                .Columns("IssuerId", "Subject");

            Create.ForeignKey()
                .FromTable("UserLogin").InSchema("Access")
                .ForeignColumn("IssuerId")
                .ToTable("Issuer").InSchema("Access")
                .PrimaryColumn("IssuerId");

            Create.ForeignKey()
                .FromTable("UserLogin").InSchema("Access")
                .ForeignColumn("UserId")
                .ToTable("User").InSchema("Access")
                .PrimaryColumn("UserId");

            Insert
                .IntoTable("Issuer").InSchema("Access")
                .Row(new { IssuerId = Guid.NewGuid(), Name = "Swetugg Local", IssuerIdentifier = _issuerIdentifier });
        }

        public override void Down()
        {
            Delete.Table("UserLogin").InSchema("Access");
            Delete.Table("Issuer").InSchema("Access");
            Delete.Table("User").InSchema("Access");
            Delete.Schema("Access");
        }
    }
}
