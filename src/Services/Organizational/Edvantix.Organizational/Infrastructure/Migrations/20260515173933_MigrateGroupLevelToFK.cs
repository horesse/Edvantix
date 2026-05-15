using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edvantix.Organizational.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MigrateGroupLevelToFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_groups_organization_id_status",
                table: "groups");

            migrationBuilder.DropColumn(
                name: "level",
                table: "groups");

            migrationBuilder.AddColumn<Guid>(
                name: "level_id",
                table: "groups",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "ix_groups_level_id",
                table: "groups",
                column: "level_id");

            migrationBuilder.CreateIndex(
                name: "ix_groups_organization_id_level_id_status",
                table: "groups",
                columns: new[] { "organization_id", "level_id", "status" });

            migrationBuilder.AddForeignKey(
                name: "fk_groups_levels_level_id",
                table: "groups",
                column: "level_id",
                principalTable: "levels",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_groups_levels_level_id",
                table: "groups");

            migrationBuilder.DropIndex(
                name: "ix_groups_level_id",
                table: "groups");

            migrationBuilder.DropIndex(
                name: "ix_groups_organization_id_level_id_status",
                table: "groups");

            migrationBuilder.DropColumn(
                name: "level_id",
                table: "groups");

            migrationBuilder.AddColumn<string>(
                name: "level",
                table: "groups",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "ix_groups_organization_id_status",
                table: "groups",
                columns: new[] { "organization_id", "status" });
        }
    }
}
