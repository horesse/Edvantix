using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edvantix.Organizational.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameLevelToneValues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Rename legacy enum member names stored as strings.
            // Red  → Rose,  Green → Emerald (added in design v2).
            migrationBuilder.Sql("UPDATE levels SET tone = 'Rose'    WHERE tone = 'Red';");
            migrationBuilder.Sql("UPDATE levels SET tone = 'Emerald' WHERE tone = 'Green';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE levels SET tone = 'Red'   WHERE tone = 'Rose';");
            migrationBuilder.Sql("UPDATE levels SET tone = 'Green' WHERE tone = 'Emerald';");
        }
    }
}
