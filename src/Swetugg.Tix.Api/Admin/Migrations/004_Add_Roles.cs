using FluentMigrator;
using Swetugg.Tix.User.Contract;
using System;

namespace Swetugg.Tix.Api.Admin.Migrations
{
    [Migration(4)]
    public class AddRoles : Migration
    {
        public override void Up()
        {
            /* Permissions */
            Create.Table("Permission")
                .InSchema("Access")
                .WithColumn("Code").AsString(100).PrimaryKey()
                .WithColumn("Description").AsString(400);

            /* Roles */
            Create.Table("Role")
                .InSchema("Access")
                .WithColumn("RoleId").AsGuid().PrimaryKey()
                .WithColumn("Name").AsString(100)
                .WithColumn("Description").AsString(400);

            Create.Table("RolePermission")
                .InSchema("Access")
                .WithColumn("RoleId").AsGuid().NotNullable()
                .WithColumn("PermissionCode").AsString(100).NotNullable();

            Create.PrimaryKey()
                .OnTable("RolePermission").WithSchema("Access")
                .Columns("RoleId", "PermissionCode");

            Create.ForeignKey()
                .FromTable("RolePermission").InSchema("Access").ForeignColumn("RoleId")
                .ToTable("Role").InSchema("Access").PrimaryColumn("RoleId");

            Create.ForeignKey()
                .FromTable("RolePermission").InSchema("Access").ForeignColumn("PermissionCode")
                .ToTable("Permission").InSchema("Access").PrimaryColumn("Code");

            /* User role assignment */
            Create.Table("UserRole")
                .InSchema("Access")
                .WithColumn("UserId").AsGuid().NotNullable()
                .WithColumn("RoleId").AsGuid().NotNullable()
                .WithColumn("Path").AsString(400)
                .WithColumn("LastUpdated").AsDateTime2().NotNullable();

            Create.PrimaryKey().OnTable("UserRole")
                .WithSchema("Access")
                .Columns("UserId", "RoleId");

            Create.ForeignKey()
                .FromTable("UserRole").InSchema("Access").ForeignColumn("UserId")
                .ToTable("User").InSchema("Access").PrimaryColumn("UserId");

            Create.ForeignKey()
                .FromTable("UserRole").InSchema("Access").ForeignColumn("RoleId")
                .ToTable("Role").InSchema("Access").PrimaryColumn("RoleId");

            Create.Table("UserRoleAttribute")
                .InSchema("Access")
                .WithColumn("UserId").AsGuid().NotNullable()
                .WithColumn("RoleId").AsGuid().NotNullable()
                .WithColumn("Key").AsString(100).NotNullable()
                .WithColumn("Value").AsString(100).NotNullable();

            Create.ForeignKey()
                .FromTable("UserRoleAttribute").InSchema("Access").ForeignColumn("UserId")
                .ToTable("User").InSchema("Access").PrimaryColumn("UserId");

            Create.ForeignKey()
                .FromTable("UserRoleAttribute").InSchema("Access").ForeignColumn("RoleId")
                .ToTable("Role").InSchema("Access").PrimaryColumn("RoleId");

            Create.ForeignKey()
                .FromTable("UserRoleAttribute").InSchema("Access").ForeignColumns("UserId", "RoleId")
                .ToTable("UserRole").InSchema("Access").PrimaryColumns("UserId", "RoleId");

            Insert.IntoTable("Permission").InSchema("Access")
                .Row(new { Code = "ListOrganizations", Description = "Can list organizations in the current scope" })
                .Row(new { Code = "ListActivities", Description = "Can list activities in the current scope" })
                .Row(new { Code = "ReadOrganizationBasic", Description = "Can read basic information about an organization" })
                .Row(new { Code = "ReadActivityBasic", Description = "Can read basic information about an activity" })
                .Row(new { Code = "CreateOrganization", Description = "Can create a new Organization" })
                .Row(new { Code = "CreateActivity", Description = "Can create an Activity" })
                ;

            CreateRole(
                "Admin", "Administrator with full access", 
                "ListOrganizations", "ListActivities", "ReadOrganizationBasic", "ReadActivityBasic", "CreateOrganization", "CreateActivity");


        }

        public Guid CreateRole(string name, string description, params string[] permissionCodes)
        {
            var roleId = Guid.NewGuid();
            Insert.IntoTable("Role").InSchema("Access")
                .Row(new { roleId, name, description });
            
            foreach(var permissionCode in permissionCodes)
            {
                Insert.IntoTable("RolePermission").InSchema("Access")
                    .Row(new { roleId, permissionCode });
            }
            return roleId;
        }

        public override void Down()
        {
            Delete.Table("UserRoleAttribute").InSchema("Access");
            Delete.Table("RolePermission").InSchema("Access");
            Delete.Table("UserRole").InSchema("Access");
            Delete.Table("Role").InSchema("Access");
            Delete.Table("Permission").InSchema("Access");
        }
    }
}
