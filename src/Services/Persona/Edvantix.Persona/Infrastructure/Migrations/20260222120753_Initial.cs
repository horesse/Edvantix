using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edvantix.Persona.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "profile",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    account_id = table.Column<Guid>(type: "uuid", nullable: false),
                    login = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    gender = table.Column<int>(type: "integer", nullable: false),
                    birth_date = table.Column<DateOnly>(type: "date", nullable: false),
                    avatar_url = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, comment: "Признак удаленной записи")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_profile", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "education",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    date_start = table.Column<DateOnly>(type: "date", nullable: false),
                    date_end = table.Column<DateOnly>(type: "date", nullable: true),
                    institution = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    specialty = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    education_level = table.Column<byte>(type: "smallint", nullable: false),
                    profile_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_education", x => x.id);
                    table.ForeignKey(
                        name: "fk_education_profile_profile_id",
                        column: x => x.profile_id,
                        principalTable: "profile",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "employment_history",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    workplace = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    position = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    start_date = table.Column<DateTime>(type: "date", nullable: false),
                    end_date = table.Column<DateTime>(type: "date", nullable: true),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    profile_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_employment_history", x => x.id);
                    table.ForeignKey(
                        name: "fk_employment_history_profile_profile_id",
                        column: x => x.profile_id,
                        principalTable: "profile",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "full_name",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    first_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    last_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    middle_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    profile_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_full_name", x => x.id);
                    table.ForeignKey(
                        name: "fk_full_name_profile_profile_id",
                        column: x => x.profile_id,
                        principalTable: "profile",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "profile_contact",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    type = table.Column<int>(type: "integer", nullable: false),
                    value = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    profile_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_profile_contact", x => x.id);
                    table.ForeignKey(
                        name: "fk_profile_contact_profile_profile_id",
                        column: x => x.profile_id,
                        principalTable: "profile",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_education_is_deleted",
                table: "education",
                column: "is_deleted");

            migrationBuilder.CreateIndex(
                name: "ix_education_profile_id",
                table: "education",
                column: "profile_id");

            migrationBuilder.CreateIndex(
                name: "ix_employment_history_profile_id",
                table: "employment_history",
                column: "profile_id");

            migrationBuilder.CreateIndex(
                name: "ix_employment_history_start_date",
                table: "employment_history",
                column: "start_date");

            migrationBuilder.CreateIndex(
                name: "ix_full_name_profile_id",
                table: "full_name",
                column: "profile_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_profile_account_id",
                table: "profile",
                column: "account_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_profile_contact_profile_id",
                table: "profile_contact",
                column: "profile_id");

            migrationBuilder.CreateIndex(
                name: "ix_profile_contact_profile_id_type_value",
                table: "profile_contact",
                columns: new[] { "profile_id", "type", "value" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "education");

            migrationBuilder.DropTable(
                name: "employment_history");

            migrationBuilder.DropTable(
                name: "full_name");

            migrationBuilder.DropTable(
                name: "profile_contact");

            migrationBuilder.DropTable(
                name: "profile");
        }
    }
}
