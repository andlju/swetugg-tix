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
                .WithColumn("PermissionCode").AsString(100).PrimaryKey()
                .WithColumn("Description").AsString(400);

            Create.Table("PermissionAttribute")
                .InSchema("Access")
                .WithColumn("PermissionCode").AsString(100).NotNullable()
                .WithColumn("Attribute").AsString(100).NotNullable();

            Create.PrimaryKey()
                .OnTable("PermissionAttribute").WithSchema("Access")
                .Columns("PermissionCode", "Attribute");

            Create.ForeignKey()
                .FromTable("PermissionAttribute").InSchema("Access").ForeignColumn("PermissionCode")
                .ToTable("Permission").InSchema("Access").PrimaryColumn("PermissionCode");

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
                .ToTable("Permission").InSchema("Access").PrimaryColumn("PermissionCode");

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
                .WithColumn("Attribute").AsString(100).NotNullable()
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


            CreatePermission("ListOrganizations", "Can list organizations in the current scope");
            CreatePermission("ListActivities", "Can list activities in the current scope", "OrganizationId");
            CreatePermission("GetOrganizationBasic", "Can read basic information about an organization", "OrganizationId");
            CreatePermission("GetActivityBasic", "Can read basic information about an activity", "OrganizationId", "ActivityId");
            CreatePermission("CreateOrganization", "Can create a new Organization");
            CreatePermission("CreateActivity", "Can create an Activity", "OrganizationId");

            CreateRole(
                "Admin", "Administrator with full access", 
                "ListOrganizations", "ListActivities", "GetOrganizationBasic", "GetActivityBasic", "CreateOrganization", "CreateActivity");

        }

        public void CreatePermission(string code, string description, params string[] attributes)
        {
            Insert.IntoTable("Permission").InSchema("Access")
                .Row(new { code, description });
            foreach(var attribute in attributes)
            {
                Insert.IntoTable("PermissionAttribute").InSchema("Access")
                    .Row(new { PermissionCode = code, attribute });
            }
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
