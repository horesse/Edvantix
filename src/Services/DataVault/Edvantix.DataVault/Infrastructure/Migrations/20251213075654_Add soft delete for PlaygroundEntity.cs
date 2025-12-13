using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edvantix.DataVault.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddsoftdeleteforPlaygroundEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "playground_entity",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                comment: "Наименование",
                oldClrType: typeof(string),
                oldType: "text",
                oldComment: "Наименование");

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "playground_entity",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "playground_entity");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "playground_entity",
                type: "text",
                nullable: false,
                comment: "Наименование",
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldComment: "Наименование");
        }
    }
}
