using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edvantix.ProfileService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Addlogin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "login",
                table: "profile",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: ""
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "login", table: "profile");
        }
    }
}
