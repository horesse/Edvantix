using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edvantix.DataVault.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixSoftDeletePlaygroundEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "is_deleted",
                table: "playground_entity",
                type: "boolean",
                nullable: false,
                comment: "Признак удаленной записи",
                oldClrType: typeof(bool),
                oldType: "boolean"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "is_deleted",
                table: "playground_entity",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldComment: "Признак удаленной записи"
            );
        }
    }
}
