using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edvantix.ProfileService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Addavatar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "avatar",
                table: "profile",
                type: "text",
                nullable: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "avatar", table: "profile");
        }
    }
}
