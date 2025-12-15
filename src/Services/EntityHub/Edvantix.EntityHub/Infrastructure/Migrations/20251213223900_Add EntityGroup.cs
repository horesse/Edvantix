using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Edvantix.EntityHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEntityGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "entity_group_id",
                table: "entity_type",
                type: "bigint",
                nullable: false,
                defaultValue: 2L,
                comment: "Идентификатор группы сущности"
            );

            migrationBuilder.AddColumn<long>(
                name: "entity_group_id1",
                table: "entity_type",
                type: "bigint",
                nullable: true
            );

            migrationBuilder.CreateTable(
                name: "entity_group",
                columns: table => new
                {
                    id = table
                        .Column<long>(type: "bigint", nullable: false, comment: "Идентификатор")
                        .Annotation(
                            "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                        ),
                    name = table.Column<string>(
                        type: "character varying(100)",
                        maxLength: 100,
                        nullable: false,
                        comment: "Наименование группы сущностей"
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_entity_group", x => x.id);
                }
            );

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

            migrationBuilder.CreateIndex(
                name: "ix_entity_type_entity_group_id",
                table: "entity_type",
                column: "entity_group_id"
            );

            migrationBuilder.CreateIndex(
                name: "ix_entity_type_entity_group_id1",
                table: "entity_type",
                column: "entity_group_id1"
            );

            migrationBuilder.AddForeignKey(
                name: "fk_entity_type_entity_group_entity_group_id",
                table: "entity_type",
                column: "entity_group_id",
                principalTable: "entity_group",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict
            );

            migrationBuilder.AddForeignKey(
                name: "fk_entity_type_entity_group_entity_group_id1",
                table: "entity_type",
                column: "entity_group_id1",
                principalTable: "entity_group",
                principalColumn: "id"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_entity_type_entity_group_entity_group_id",
                table: "entity_type"
            );

            migrationBuilder.DropForeignKey(
                name: "fk_entity_type_entity_group_entity_group_id1",
                table: "entity_type"
            );

            migrationBuilder.DropTable(name: "entity_group");

            migrationBuilder.DropIndex(
                name: "ix_entity_type_entity_group_id",
                table: "entity_type"
            );

            migrationBuilder.DropIndex(
                name: "ix_entity_type_entity_group_id1",
                table: "entity_type"
            );

            migrationBuilder.DropColumn(name: "entity_group_id", table: "entity_type");

            migrationBuilder.DropColumn(name: "entity_group_id1", table: "entity_type");
        }
    }
}
