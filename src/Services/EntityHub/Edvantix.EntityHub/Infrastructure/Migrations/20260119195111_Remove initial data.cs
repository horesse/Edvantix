using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Edvantix.EntityHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Removeinitialdata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(table: "entity_group", keyColumn: "id", keyValue: 1L);

            migrationBuilder.DeleteData(table: "entity_group", keyColumn: "id", keyValue: 2L);

            migrationBuilder.DeleteData(table: "entity_group", keyColumn: "id", keyValue: 3L);

            migrationBuilder.DeleteData(table: "entity_group", keyColumn: "id", keyValue: 4L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "entity_group",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 1L, "System" },
                    { 2L, "Hidden" },
                    { 3L, "Other" },
                    { 4L, "Reference" },
                }
            );
        }
    }
}
