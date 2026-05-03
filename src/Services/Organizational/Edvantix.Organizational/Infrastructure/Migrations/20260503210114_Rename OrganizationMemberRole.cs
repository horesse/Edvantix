using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edvantix.Organizational.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameOrganizationMemberRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_organization_members_organization_member_roles_organization",
                table: "organization_members"
            );

            migrationBuilder.DropTable(name: "organization_member_role_permission");

            migrationBuilder.DropTable(name: "organization_member_roles");

            migrationBuilder.RenameColumn(
                name: "organization_member_role_id",
                table: "organization_members",
                newName: "organization_role_id"
            );

            migrationBuilder.RenameIndex(
                name: "ix_organization_members_organization_member_role_id",
                table: "organization_members",
                newName: "ix_organization_members_organization_role_id"
            );

            migrationBuilder.CreateTable(
                name: "organization_roles",
                columns: table => new
                {
                    id = table.Column<Guid>(
                        type: "uuid",
                        nullable: false,
                        defaultValueSql: "uuidv7()"
                    ),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(
                        type: "character varying(50)",
                        maxLength: 50,
                        nullable: false
                    ),
                    description = table.Column<string>(
                        type: "character varying(100)",
                        maxLength: 100,
                        nullable: true
                    ),
                    is_system = table.Column<bool>(type: "boolean", nullable: false),
                    is_owner = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(
                        type: "boolean",
                        nullable: false,
                        comment: "Признак удаленной записи"
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_organization_roles", x => x.id);
                }
            );

            migrationBuilder.CreateTable(
                name: "organization_role_permission",
                columns: table => new
                {
                    organization_role_id = table.Column<Guid>(type: "uuid", nullable: false),
                    permission_id = table.Column<Guid>(type: "uuid", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "pk_organization_role_permission",
                        x => new { x.organization_role_id, x.permission_id }
                    );
                    table.ForeignKey(
                        name: "fk_organization_role_permission_organization_roles_organizatio",
                        column: x => x.organization_role_id,
                        principalTable: "organization_roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "fk_organization_role_permission_permissions_permission_id",
                        column: x => x.permission_id,
                        principalTable: "permissions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "ix_organization_role_permission_permission_id",
                table: "organization_role_permission",
                column: "permission_id"
            );

            migrationBuilder.CreateIndex(
                name: "ix_organization_roles_organization_id_name",
                table: "organization_roles",
                columns: new[] { "organization_id", "name" },
                unique: true
            );

            migrationBuilder.AddForeignKey(
                name: "fk_organization_members_organization_roles_organization_role_id",
                table: "organization_members",
                column: "organization_role_id",
                principalTable: "organization_roles",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_organization_members_organization_roles_organization_role_id",
                table: "organization_members"
            );

            migrationBuilder.DropTable(name: "organization_role_permission");

            migrationBuilder.DropTable(name: "organization_roles");

            migrationBuilder.RenameColumn(
                name: "organization_role_id",
                table: "organization_members",
                newName: "organization_member_role_id"
            );

            migrationBuilder.RenameIndex(
                name: "ix_organization_members_organization_role_id",
                table: "organization_members",
                newName: "ix_organization_members_organization_member_role_id"
            );

            migrationBuilder.CreateTable(
                name: "organization_member_roles",
                columns: table => new
                {
                    id = table.Column<Guid>(
                        type: "uuid",
                        nullable: false,
                        defaultValueSql: "uuidv7()"
                    ),
                    description = table.Column<string>(
                        type: "character varying(100)",
                        maxLength: 100,
                        nullable: true
                    ),
                    is_deleted = table.Column<bool>(
                        type: "boolean",
                        nullable: false,
                        comment: "Признак удаленной записи"
                    ),
                    is_owner = table.Column<bool>(type: "boolean", nullable: false),
                    is_system = table.Column<bool>(type: "boolean", nullable: false),
                    name = table.Column<string>(
                        type: "character varying(50)",
                        maxLength: 50,
                        nullable: false
                    ),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_organization_member_roles", x => x.id);
                }
            );

            migrationBuilder.CreateTable(
                name: "organization_member_role_permission",
                columns: table => new
                {
                    organization_member_role_id = table.Column<Guid>(type: "uuid", nullable: false),
                    permission_id = table.Column<Guid>(type: "uuid", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "pk_organization_member_role_permission",
                        x => new { x.organization_member_role_id, x.permission_id }
                    );
                    table.ForeignKey(
                        name: "fk_organization_member_role_permission_organization_member_rol",
                        column: x => x.organization_member_role_id,
                        principalTable: "organization_member_roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "fk_organization_member_role_permission_permissions_permission_",
                        column: x => x.permission_id,
                        principalTable: "permissions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "ix_organization_member_role_permission_permission_id",
                table: "organization_member_role_permission",
                column: "permission_id"
            );

            migrationBuilder.CreateIndex(
                name: "ix_organization_member_roles_organization_id_name",
                table: "organization_member_roles",
                columns: new[] { "organization_id", "name" },
                unique: true
            );

            migrationBuilder.AddForeignKey(
                name: "fk_organization_members_organization_member_roles_organization",
                table: "organization_members",
                column: "organization_member_role_id",
                principalTable: "organization_member_roles",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict
            );
        }
    }
}
