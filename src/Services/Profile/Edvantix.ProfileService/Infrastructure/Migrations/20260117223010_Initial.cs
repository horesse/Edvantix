using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Edvantix.ProfileService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                    name = table.Column<string>(
                        type: "character varying(100)",
                        maxLength: 100,
                        nullable: false
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
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_education_level", x => x.id);
                }
            );

            migrationBuilder.CreateTable(
                name: "profile",
                columns: table => new
                {
                    id = table
                        .Column<long>(type: "bigint", nullable: false, comment: "Идентификатор")
                        .Annotation(
                            "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                        ),
                    account_id = table.Column<Guid>(type: "uuid", nullable: false),
                    gender = table.Column<int>(type: "integer", nullable: false),
                    is_deleted = table.Column<bool>(
                        type: "boolean",
                        nullable: false,
                        comment: "Признак удаленной записи"
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_profile", x => x.id);
                }
            );

            migrationBuilder.CreateTable(
                name: "education",
                columns: table => new
                {
                    id = table
                        .Column<long>(type: "bigint", nullable: false, comment: "Идентификатор")
                        .Annotation(
                            "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                        ),
                    date_start = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    date_end = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    institution = table.Column<string>(
                        type: "character varying(255)",
                        maxLength: 255,
                        nullable: false
                    ),
                    specialty = table.Column<string>(
                        type: "character varying(100)",
                        maxLength: 100,
                        nullable: true
                    ),
                    education_level_id = table.Column<long>(type: "bigint", nullable: false),
                    is_deleted = table.Column<bool>(
                        type: "boolean",
                        nullable: false,
                        comment: "Признак удаленной записи"
                    ),
                    profile_id = table.Column<long>(type: "bigint", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_education", x => x.id);
                    table.ForeignKey(
                        name: "fk_education_education_level_education_level_id",
                        column: x => x.education_level_id,
                        principalTable: "education_level",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict
                    );
                    table.ForeignKey(
                        name: "fk_education_profile_profile_id",
                        column: x => x.profile_id,
                        principalTable: "profile",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "employment_history",
                columns: table => new
                {
                    id = table
                        .Column<long>(type: "bigint", nullable: false, comment: "Идентификатор")
                        .Annotation(
                            "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                        ),
                    workplace = table.Column<string>(
                        type: "character varying(255)",
                        maxLength: 255,
                        nullable: false
                    ),
                    position = table.Column<string>(
                        type: "character varying(255)",
                        maxLength: 255,
                        nullable: false
                    ),
                    start_date = table.Column<DateTime>(type: "date", nullable: false),
                    end_date = table.Column<DateTime>(type: "date", nullable: true),
                    description = table.Column<string>(
                        type: "character varying(1000)",
                        maxLength: 1000,
                        nullable: true
                    ),
                    is_deleted = table.Column<bool>(
                        type: "boolean",
                        nullable: false,
                        defaultValue: false,
                        comment: "Признак удаленной записи"
                    ),
                    profile_id = table.Column<long>(type: "bigint", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_employment_history", x => x.id);
                    table.ForeignKey(
                        name: "fk_employment_history_profile_profile_id",
                        column: x => x.profile_id,
                        principalTable: "profile",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "full_name",
                columns: table => new
                {
                    id = table
                        .Column<long>(type: "bigint", nullable: false, comment: "Идентификатор")
                        .Annotation(
                            "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                        ),
                    first_name = table.Column<string>(
                        type: "character varying(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    last_name = table.Column<string>(
                        type: "character varying(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    middle_name = table.Column<string>(
                        type: "character varying(100)",
                        maxLength: 100,
                        nullable: true
                    ),
                    is_deleted = table.Column<bool>(
                        type: "boolean",
                        nullable: false,
                        comment: "Признак удаленной записи"
                    ),
                    profile_id = table.Column<long>(type: "bigint", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_full_name", x => x.id);
                    table.ForeignKey(
                        name: "fk_full_name_profile_profile_id",
                        column: x => x.profile_id,
                        principalTable: "profile",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "user_contact",
                columns: table => new
                {
                    id = table
                        .Column<long>(type: "bigint", nullable: false, comment: "Идентификатор")
                        .Annotation(
                            "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                        ),
                    type = table.Column<int>(type: "integer", nullable: false),
                    value = table.Column<string>(
                        type: "character varying(255)",
                        maxLength: 255,
                        nullable: false
                    ),
                    description = table.Column<string>(
                        type: "character varying(500)",
                        maxLength: 500,
                        nullable: true
                    ),
                    is_deleted = table.Column<bool>(
                        type: "boolean",
                        nullable: false,
                        defaultValue: false,
                        comment: "Признак удаленной записи"
                    ),
                    profile_id = table.Column<long>(type: "bigint", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_contact", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_contact_profile_profile_id",
                        column: x => x.profile_id,
                        principalTable: "profile",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
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
                name: "ix_education_is_deleted",
                table: "education",
                column: "is_deleted"
            );

            migrationBuilder.CreateIndex(
                name: "ix_education_profile_id",
                table: "education",
                column: "profile_id"
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

            migrationBuilder.CreateIndex(
                name: "ix_employment_history_profile_id",
                table: "employment_history",
                column: "profile_id"
            );

            migrationBuilder.CreateIndex(
                name: "ix_employment_history_start_date",
                table: "employment_history",
                column: "start_date"
            );

            migrationBuilder.CreateIndex(
                name: "ix_full_name_profile_id",
                table: "full_name",
                column: "profile_id",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "ix_profile_account_id",
                table: "profile",
                column: "account_id",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "ix_user_contact_profile_id",
                table: "user_contact",
                column: "profile_id"
            );

            migrationBuilder.CreateIndex(
                name: "ix_user_contact_profile_id_type_value",
                table: "user_contact",
                columns: new[] { "profile_id", "type", "value" }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "education");

            migrationBuilder.DropTable(name: "employment_history");

            migrationBuilder.DropTable(name: "full_name");

            migrationBuilder.DropTable(name: "user_contact");

            migrationBuilder.DropTable(name: "education_level");

            migrationBuilder.DropTable(name: "profile");
        }
    }
}
