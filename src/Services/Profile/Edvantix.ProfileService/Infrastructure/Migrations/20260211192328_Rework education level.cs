using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Edvantix.ProfileService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Reworkeducationlevel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_education_education_level_education_level_id",
                table: "education"
            );

            migrationBuilder.DropTable(name: "education_level");

            migrationBuilder.DropIndex(name: "ix_education_education_level_id", table: "education");

            migrationBuilder.DropColumn(name: "education_level_id", table: "education");

            migrationBuilder.AddColumn<byte>(
                name: "education_level",
                table: "education",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "education_level", table: "education");

            migrationBuilder.AddColumn<long>(
                name: "education_level_id",
                table: "education",
                type: "bigint",
                nullable: false,
                defaultValue: 0L
            );

            migrationBuilder.CreateTable(
                name: "education_level",
                columns: table => new
                {
                    id = table
                        .Column<long>(type: "bigint", nullable: false, comment: "Идентификатор")
                        .Annotation(
                            "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                        ),
                    code = table.Column<string>(
                        type: "character varying(50)",
                        maxLength: 50,
                        nullable: false
                    ),
                    is_deleted = table.Column<bool>(
                        type: "boolean",
                        nullable: false,
                        comment: "Признак удаленной записи"
                    ),
                    name = table.Column<string>(
                        type: "character varying(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_education_level", x => x.id);
                }
            );

            migrationBuilder.InsertData(
                table: "education_level",
                columns: new[] { "id", "code", "is_deleted", "name" },
                values: new object[,]
                {
                    { 1L, "preschool", false, "Дошкольное образование" },
                    { 2L, "general_secondary", false, "Общее среднее образование" },
                    {
                        3L,
                        "vocational_technical",
                        false,
                        "Профессионально-техническое образование",
                    },
                    { 4L, "secondary_specialized", false, "Среднее специальное образование" },
                    { 5L, "higher_bachelor", false, "Высшее образование (I ступень)" },
                    { 6L, "higher_master", false, "Высшее образование (II ступень)" },
                    { 7L, "postgraduate", false, "Послевузовское образование" },
                    {
                        8L,
                        "additional_children",
                        false,
                        "Дополнительное образование детей и молодежи",
                    },
                    { 9L, "additional_adults", false, "Дополнительное образование взрослых" },
                    { 10L, "special", false, "Специальное образование" },
                }
            );

            migrationBuilder.CreateIndex(
                name: "ix_education_education_level_id",
                table: "education",
                column: "education_level_id"
            );

            migrationBuilder.CreateIndex(
                name: "ix_education_level_code",
                table: "education_level",
                column: "code"
            );

            migrationBuilder.CreateIndex(
                name: "ix_education_level_is_deleted",
                table: "education_level",
                column: "is_deleted"
            );

            migrationBuilder.AddForeignKey(
                name: "fk_education_education_level_education_level_id",
                table: "education",
                column: "education_level_id",
                principalTable: "education_level",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict
            );
        }
    }
}
