using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edvantix.Organizational.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGroupFieldsAndRooms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "status",
                table: "group_members",
                newName: "role");

            migrationBuilder.AddColumn<int>(
                name: "capacity",
                table: "groups",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "code",
                table: "groups",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "course_id",
                table: "groups",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "format",
                table: "groups",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "level",
                table: "groups",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "platform",
                table: "groups",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "room_id",
                table: "groups",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "teacher_member_id",
                table: "groups",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "rooms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    label = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    floor = table.Column<short>(type: "smallint", nullable: false),
                    seats = table.Column<short>(type: "smallint", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, comment: "Признак удаленной записи")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_rooms", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_groups_course_id",
                table: "groups",
                column: "course_id");

            migrationBuilder.CreateIndex(
                name: "ix_groups_organization_id_code",
                table: "groups",
                columns: new[] { "organization_id", "code" },
                unique: true,
                filter: "is_deleted = false");

            migrationBuilder.CreateIndex(
                name: "ix_groups_organization_id_status",
                table: "groups",
                columns: new[] { "organization_id", "status" });

            migrationBuilder.CreateIndex(
                name: "ix_groups_teacher_member_id",
                table: "groups",
                column: "teacher_member_id");

            migrationBuilder.CreateIndex(
                name: "ix_rooms_organization_id",
                table: "rooms",
                column: "organization_id",
                filter: "is_deleted = false");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "rooms");

            migrationBuilder.DropIndex(
                name: "ix_groups_course_id",
                table: "groups");

            migrationBuilder.DropIndex(
                name: "ix_groups_organization_id_code",
                table: "groups");

            migrationBuilder.DropIndex(
                name: "ix_groups_organization_id_status",
                table: "groups");

            migrationBuilder.DropIndex(
                name: "ix_groups_teacher_member_id",
                table: "groups");

            migrationBuilder.DropColumn(
                name: "capacity",
                table: "groups");

            migrationBuilder.DropColumn(
                name: "code",
                table: "groups");

            migrationBuilder.DropColumn(
                name: "course_id",
                table: "groups");

            migrationBuilder.DropColumn(
                name: "format",
                table: "groups");

            migrationBuilder.DropColumn(
                name: "level",
                table: "groups");

            migrationBuilder.DropColumn(
                name: "platform",
                table: "groups");

            migrationBuilder.DropColumn(
                name: "room_id",
                table: "groups");

            migrationBuilder.DropColumn(
                name: "teacher_member_id",
                table: "groups");

            migrationBuilder.RenameColumn(
                name: "role",
                table: "group_members",
                newName: "status");
        }
    }
}
