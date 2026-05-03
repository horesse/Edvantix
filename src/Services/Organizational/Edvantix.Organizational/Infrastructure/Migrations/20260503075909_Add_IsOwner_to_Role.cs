using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edvantix.Organizational.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Add_IsOwner_to_Role : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "code",
                table: "organization_member_roles",
                newName: "name"
            );

            migrationBuilder.RenameIndex(
                name: "ix_organization_member_roles_organization_id_code",
                table: "organization_member_roles",
                newName: "ix_organization_member_roles_organization_id_name"
            );

            migrationBuilder.AddColumn<bool>(
                name: "is_owner",
                table: "organization_member_roles",
                type: "boolean",
                nullable: false,
                defaultValue: false
            );

            migrationBuilder.AddColumn<bool>(
                name: "is_system",
                table: "organization_member_roles",
                type: "boolean",
                nullable: false,
                defaultValue: false
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "is_owner", table: "organization_member_roles");

            migrationBuilder.DropColumn(name: "is_system", table: "organization_member_roles");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "organization_member_roles",
                newName: "code"
            );

            migrationBuilder.RenameIndex(
                name: "ix_organization_member_roles_organization_id_name",
                table: "organization_member_roles",
                newName: "ix_organization_member_roles_organization_id_code"
            );
        }
    }
}
