using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Edvantix.EntityHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "microservice",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "Идентификатор")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false, comment: "Наименование сервиса")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_microservice", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "entity_type",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "Идентификатор")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false, comment: "Наименование сущности"),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true, comment: "Описание сущности"),
                    microservice_id = table.Column<long>(type: "bigint", nullable: false, comment: "Идентификатор микросервиса")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_entity_type", x => x.id);
                    table.ForeignKey(
                        name: "fk_entity_type_microservice_microservice_id",
                        column: x => x.microservice_id,
                        principalTable: "microservice",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_entity_type_microservice_id",
                table: "entity_type",
                column: "microservice_id");

            migrationBuilder.CreateIndex(
                name: "ix_microservice_name",
                table: "microservice",
                column: "name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "entity_type");

            migrationBuilder.DropTable(
                name: "microservice");
        }
    }
}
