using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edvantix.Organizational.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Addautoincludes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "is_deleted",
                table: "organizations",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldComment: "Признак удаленной записи"
            );

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "organizations",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "NOW() AT TIME ZONE 'UTC'"
            );

            migrationBuilder.AddColumn<DateTime>(
                name: "last_modified_at",
                table: "organizations",
                type: "timestamp with time zone",
                nullable: true,
                defaultValueSql: "NOW() AT TIME ZONE 'UTC'"
            );

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "organizations",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "created_at", table: "organizations");

            migrationBuilder.DropColumn(name: "last_modified_at", table: "organizations");

            migrationBuilder.DropColumn(name: "xmin", table: "organizations");

            migrationBuilder.AlterColumn<bool>(
                name: "is_deleted",
                table: "organizations",
                type: "boolean",
                nullable: false,
                comment: "Признак удаленной записи",
                oldClrType: typeof(bool),
                oldType: "boolean"
            );
        }
    }
}
