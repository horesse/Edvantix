using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edvantix.EntityHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixEntityGroupId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_entity_type_entity_group_entity_group_id1",
                table: "entity_type");

            migrationBuilder.DropIndex(
                name: "ix_entity_type_entity_group_id1",
                table: "entity_type");

            migrationBuilder.DropColumn(
                name: "entity_group_id1",
                table: "entity_type");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "entity_group_id1",
                table: "entity_type",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_entity_type_entity_group_id1",
                table: "entity_type",
                column: "entity_group_id1");

            migrationBuilder.AddForeignKey(
                name: "fk_entity_type_entity_group_entity_group_id1",
                table: "entity_type",
                column: "entity_group_id1",
                principalTable: "entity_group",
                principalColumn: "id");
        }
    }
}
