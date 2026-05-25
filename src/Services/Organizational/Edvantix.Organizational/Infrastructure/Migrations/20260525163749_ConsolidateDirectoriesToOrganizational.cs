using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edvantix.Organizational.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ConsolidateDirectoriesToOrganizational : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "lesson_types",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    default_duration_minutes = table.Column<int>(type: "integer", nullable: false),
                    color = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false),
                    icon = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_modified_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    row_version = table.Column<long>(type: "bigint", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    is_archived = table.Column<bool>(type: "boolean", nullable: false),
                    order = table.Column<int>(type: "integer", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    last_modified_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_lesson_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "levels",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    description = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    tone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    sort_order = table.Column<short>(type: "smallint", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, comment: "Признак удаленной записи")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_levels", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "subjects",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    color = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW() AT TIME ZONE 'UTC'"),
                    last_modified_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "NOW() AT TIME ZONE 'UTC'"),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    is_archived = table.Column<bool>(type: "boolean", nullable: false),
                    order = table.Column<int>(type: "integer", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    last_modified_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_subjects", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_lesson_types_organization_id_code",
                table: "lesson_types",
                columns: new[] { "organization_id", "code" },
                unique: true,
                filter: "is_archived = false");

            migrationBuilder.CreateIndex(
                name: "ix_lesson_types_organization_id_is_archived",
                table: "lesson_types",
                columns: new[] { "organization_id", "is_archived" });

            migrationBuilder.CreateIndex(
                name: "ix_lesson_types_organization_id_name",
                table: "lesson_types",
                columns: new[] { "organization_id", "name" },
                unique: true,
                filter: "is_archived = false");

            migrationBuilder.CreateIndex(
                name: "ix_levels_organization_id_code",
                table: "levels",
                columns: new[] { "organization_id", "code" },
                unique: true,
                filter: "is_deleted = false");

            migrationBuilder.CreateIndex(
                name: "ix_levels_organization_id_is_active",
                table: "levels",
                columns: new[] { "organization_id", "is_active" });

            migrationBuilder.CreateIndex(
                name: "ix_levels_organization_id_sort_order",
                table: "levels",
                columns: new[] { "organization_id", "sort_order" },
                unique: true,
                filter: "is_deleted = false");

            migrationBuilder.CreateIndex(
                name: "ix_subjects_organization_id_code",
                table: "subjects",
                columns: new[] { "organization_id", "code" },
                unique: true,
                filter: "is_archived = false");

            migrationBuilder.CreateIndex(
                name: "ix_subjects_organization_id_is_archived",
                table: "subjects",
                columns: new[] { "organization_id", "is_archived" });

            migrationBuilder.CreateIndex(
                name: "ix_subjects_organization_id_name",
                table: "subjects",
                columns: new[] { "organization_id", "name" },
                unique: true,
                filter: "is_archived = false");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "lesson_types");

            migrationBuilder.DropTable(
                name: "levels");

            migrationBuilder.DropTable(
                name: "subjects");
        }
    }
}
