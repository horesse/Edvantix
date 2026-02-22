using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edvantix.Organizational.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "invitation",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    invited_by_profile_id = table.Column<Guid>(type: "uuid", nullable: false),
                    invitee_profile_id = table.Column<Guid>(type: "uuid", nullable: true),
                    invitee_email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    role = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    token = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    responded_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_invitation", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "organization",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    name_latin = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    short_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    print_name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    description = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: true),
                    registration_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_organization", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "group",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    description = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, comment: "Признак удаленной записи")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_group", x => x.id);
                    table.ForeignKey(
                        name: "fk_group_organization_organization_id",
                        column: x => x.organization_id,
                        principalTable: "organization",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "organization_contact",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    value = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_organization_contact", x => x.id);
                    table.ForeignKey(
                        name: "fk_organization_contact_organization_organization_id",
                        column: x => x.organization_id,
                        principalTable: "organization",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "organization_member",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    profile_id = table.Column<Guid>(type: "uuid", nullable: false),
                    role = table.Column<int>(type: "integer", nullable: false),
                    joined_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, comment: "Признак удаленной записи")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_organization_member", x => x.id);
                    table.ForeignKey(
                        name: "fk_organization_member_organization_organization_id",
                        column: x => x.organization_id,
                        principalTable: "organization",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "group_member",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    group_id = table.Column<Guid>(type: "uuid", nullable: false),
                    profile_id = table.Column<Guid>(type: "uuid", nullable: false),
                    role = table.Column<int>(type: "integer", nullable: false),
                    joined_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, comment: "Признак удаленной записи")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_group_member", x => x.id);
                    table.ForeignKey(
                        name: "fk_group_member_group_group_id",
                        column: x => x.group_id,
                        principalTable: "group",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_group_name",
                table: "group",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "ix_group_organization_id",
                table: "group",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "ix_group_member_group_id",
                table: "group_member",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "ix_group_member_group_id_profile_id_is_deleted",
                table: "group_member",
                columns: new[] { "group_id", "profile_id", "is_deleted" });

            migrationBuilder.CreateIndex(
                name: "ix_group_member_profile_id",
                table: "group_member",
                column: "profile_id");

            migrationBuilder.CreateIndex(
                name: "ix_invitation_invitee_profile_id_status",
                table: "invitation",
                columns: new[] { "invitee_profile_id", "status" });

            migrationBuilder.CreateIndex(
                name: "ix_invitation_organization_id_invitee_email_status",
                table: "invitation",
                columns: new[] { "organization_id", "invitee_email", "status" });

            migrationBuilder.CreateIndex(
                name: "ix_invitation_organization_id_status",
                table: "invitation",
                columns: new[] { "organization_id", "status" });

            migrationBuilder.CreateIndex(
                name: "ix_invitation_token",
                table: "invitation",
                column: "token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_organization_name",
                table: "organization",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "ix_organization_name_latin",
                table: "organization",
                column: "name_latin");

            migrationBuilder.CreateIndex(
                name: "ix_organization_registration_date",
                table: "organization",
                column: "registration_date");

            migrationBuilder.CreateIndex(
                name: "ix_organization_short_name",
                table: "organization",
                column: "short_name");

            migrationBuilder.CreateIndex(
                name: "ix_organization_contact_organization_id",
                table: "organization_contact",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "ix_organization_contact_organization_id_type",
                table: "organization_contact",
                columns: new[] { "organization_id", "type" });

            migrationBuilder.CreateIndex(
                name: "ix_organization_contact_type",
                table: "organization_contact",
                column: "type");

            migrationBuilder.CreateIndex(
                name: "ix_organization_member_organization_id",
                table: "organization_member",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "ix_organization_member_organization_id_profile_id_is_deleted",
                table: "organization_member",
                columns: new[] { "organization_id", "profile_id", "is_deleted" });

            migrationBuilder.CreateIndex(
                name: "ix_organization_member_profile_id",
                table: "organization_member",
                column: "profile_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "group_member");

            migrationBuilder.DropTable(
                name: "invitation");

            migrationBuilder.DropTable(
                name: "organization_contact");

            migrationBuilder.DropTable(
                name: "organization_member");

            migrationBuilder.DropTable(
                name: "group");

            migrationBuilder.DropTable(
                name: "organization");
        }
    }
}
