using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edvantix.Organizational.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RebuildRoomsAsDirectoryLookup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_rooms_organization_id",
                table: "rooms");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "rooms");

            migrationBuilder.DropColumn(
                name: "label",
                table: "rooms");

            migrationBuilder.DropColumn(
                name: "seats",
                table: "rooms");

            migrationBuilder.AlterColumn<string>(
                name: "floor",
                table: "rooms",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.AddColumn<int>(
                name: "capacity",
                table: "rooms",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "rooms",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "created_by",
                table: "rooms",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_archived",
                table: "rooms",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "last_modified_at",
                table: "rooms",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "last_modified_by",
                table: "rooms",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "name",
                table: "rooms",
                type: "character varying(120)",
                maxLength: 120,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "order",
                table: "rooms",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "room_type",
                table: "rooms",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "row_version",
                table: "rooms",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "ix_rooms_org_id",
                table: "rooms",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "ix_rooms_org_name_active",
                table: "rooms",
                columns: new[] { "organization_id", "name" },
                unique: true,
                filter: "is_archived = false");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_rooms_org_id",
                table: "rooms");

            migrationBuilder.DropIndex(
                name: "ix_rooms_org_name_active",
                table: "rooms");

            migrationBuilder.DropColumn(
                name: "capacity",
                table: "rooms");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "rooms");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "rooms");

            migrationBuilder.DropColumn(
                name: "is_archived",
                table: "rooms");

            migrationBuilder.DropColumn(
                name: "last_modified_at",
                table: "rooms");

            migrationBuilder.DropColumn(
                name: "last_modified_by",
                table: "rooms");

            migrationBuilder.DropColumn(
                name: "name",
                table: "rooms");

            migrationBuilder.DropColumn(
                name: "order",
                table: "rooms");

            migrationBuilder.DropColumn(
                name: "room_type",
                table: "rooms");

            migrationBuilder.DropColumn(
                name: "row_version",
                table: "rooms");

            migrationBuilder.AlterColumn<short>(
                name: "floor",
                table: "rooms",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "rooms",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                comment: "Признак удаленной записи");

            migrationBuilder.AddColumn<string>(
                name: "label",
                table: "rooms",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<short>(
                name: "seats",
                table: "rooms",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.CreateIndex(
                name: "ix_rooms_organization_id",
                table: "rooms",
                column: "organization_id",
                filter: "is_deleted = false");
        }
    }
}
