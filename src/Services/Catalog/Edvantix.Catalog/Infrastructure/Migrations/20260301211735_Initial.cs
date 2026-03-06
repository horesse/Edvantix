using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edvantix.Catalog.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "currencies",
                columns: table => new
                {
                    code = table.Column<string>(
                        type: "character varying(5)",
                        maxLength: 5,
                        nullable: false
                    ),
                    name_ru = table.Column<string>(
                        type: "character varying(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    name_en = table.Column<string>(
                        type: "character varying(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    symbol = table.Column<string>(
                        type: "character varying(5)",
                        maxLength: 5,
                        nullable: false
                    ),
                    numeric_code = table.Column<int>(type: "integer", nullable: false),
                    decimal_digits = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_currencies", x => x.code);
                }
            );

            migrationBuilder.CreateTable(
                name: "languages",
                columns: table => new
                {
                    code = table.Column<string>(
                        type: "character varying(5)",
                        maxLength: 5,
                        nullable: false
                    ),
                    name_ru = table.Column<string>(
                        type: "character varying(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    name_en = table.Column<string>(
                        type: "character varying(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    native_name = table.Column<string>(
                        type: "character varying(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_languages", x => x.code);
                }
            );

            migrationBuilder.CreateTable(
                name: "regions",
                columns: table => new
                {
                    code = table.Column<string>(
                        type: "character varying(20)",
                        maxLength: 20,
                        nullable: false
                    ),
                    name_ru = table.Column<string>(
                        type: "character varying(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    name_en = table.Column<string>(
                        type: "character varying(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_regions", x => x.code);
                }
            );

            migrationBuilder.CreateTable(
                name: "timezones",
                columns: table => new
                {
                    code = table.Column<string>(
                        type: "character varying(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    name_ru = table.Column<string>(
                        type: "character varying(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    name_en = table.Column<string>(
                        type: "character varying(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    display_name = table.Column<string>(
                        type: "character varying(255)",
                        maxLength: 255,
                        nullable: false
                    ),
                    utc_offset_minutes = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_timezones", x => x.code);
                }
            );

            migrationBuilder.CreateTable(
                name: "countries",
                columns: table => new
                {
                    code = table.Column<string>(
                        type: "character varying(5)",
                        maxLength: 5,
                        nullable: false
                    ),
                    alpha3code = table.Column<string>(
                        type: "character varying(5)",
                        maxLength: 5,
                        nullable: false
                    ),
                    name_ru = table.Column<string>(
                        type: "character varying(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    name_en = table.Column<string>(
                        type: "character varying(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    numeric_code = table.Column<int>(type: "integer", nullable: false),
                    phone_prefix = table.Column<string>(
                        type: "character varying(5)",
                        maxLength: 5,
                        nullable: false
                    ),
                    currency_code = table.Column<string>(
                        type: "character varying(5)",
                        maxLength: 5,
                        nullable: false
                    ),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_countries", x => x.code);
                    table.ForeignKey(
                        name: "fk_countries_currencies_currency_code",
                        column: x => x.currency_code,
                        principalTable: "currencies",
                        principalColumn: "code",
                        onDelete: ReferentialAction.Restrict
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "ix_countries_alpha3code",
                table: "countries",
                column: "alpha3code",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "ix_countries_currency_code",
                table: "countries",
                column: "currency_code"
            );

            migrationBuilder.CreateIndex(
                name: "ix_countries_name_en",
                table: "countries",
                column: "name_en"
            );

            migrationBuilder.CreateIndex(
                name: "ix_countries_numeric_code",
                table: "countries",
                column: "numeric_code",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "ix_currencies_numeric_code",
                table: "currencies",
                column: "numeric_code",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "ix_languages_name_en",
                table: "languages",
                column: "name_en"
            );

            migrationBuilder.CreateIndex(
                name: "ix_regions_name_en",
                table: "regions",
                column: "name_en"
            );

            migrationBuilder.CreateIndex(
                name: "ix_timezones_name_en",
                table: "timezones",
                column: "name_en"
            );

            migrationBuilder.CreateIndex(
                name: "ix_timezones_utc_offset_minutes",
                table: "timezones",
                column: "utc_offset_minutes"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "countries");

            migrationBuilder.DropTable(name: "languages");

            migrationBuilder.DropTable(name: "regions");

            migrationBuilder.DropTable(name: "timezones");

            migrationBuilder.DropTable(name: "currencies");
        }
    }
}
