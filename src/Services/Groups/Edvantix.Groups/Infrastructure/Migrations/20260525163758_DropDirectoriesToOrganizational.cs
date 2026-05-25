using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edvantix.Groups.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DropDirectoriesToOrganizational : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "fk_groups_levels_level_id", table: "groups");

            migrationBuilder.DropTable(name: "lesson_types");

            migrationBuilder.DropTable(name: "levels");

            migrationBuilder.DropTable(name: "subjects");

            migrationBuilder.DropIndex(name: "ix_groups_level_id", table: "groups");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "lesson_types",
                columns: table => new
                {
                    id = table.Column<Guid>(
                        type: "uuid",
                        nullable: false,
                        defaultValueSql: "uuidv7()"
                    ),
                    code = table.Column<string>(
                        type: "character varying(20)",
                        maxLength: 20,
                        nullable: false
                    ),
                    color = table.Column<string>(
                        type: "character varying(7)",
                        maxLength: 7,
                        nullable: false
                    ),
                    created_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    default_duration_minutes = table.Column<int>(type: "integer", nullable: false),
                    icon = table.Column<string>(
                        type: "character varying(40)",
                        maxLength: 40,
                        nullable: true
                    ),
                    is_archived = table.Column<bool>(type: "boolean", nullable: false),
                    last_modified_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    last_modified_by = table.Column<Guid>(type: "uuid", nullable: true),
                    name = table.Column<string>(
                        type: "character varying(120)",
                        maxLength: 120,
                        nullable: false
                    ),
                    order = table.Column<int>(type: "integer", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    row_version = table.Column<long>(type: "bigint", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_lesson_types", x => x.id);
                }
            );

            migrationBuilder.CreateTable(
                name: "levels",
                columns: table => new
                {
                    id = table.Column<Guid>(
                        type: "uuid",
                        nullable: false,
                        defaultValueSql: "uuidv7()"
                    ),
                    code = table.Column<string>(
                        type: "character varying(16)",
                        maxLength: 16,
                        nullable: false
                    ),
                    description = table.Column<string>(
                        type: "character varying(256)",
                        maxLength: 256,
                        nullable: true
                    ),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(
                        type: "boolean",
                        nullable: false,
                        comment: "Признак удаленной записи"
                    ),
                    name = table.Column<string>(
                        type: "character varying(64)",
                        maxLength: 64,
                        nullable: false
                    ),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    sort_order = table.Column<short>(type: "smallint", nullable: false),
                    tone = table.Column<string>(
                        type: "character varying(20)",
                        maxLength: 20,
                        nullable: false
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_levels", x => x.id);
                }
            );

            migrationBuilder.CreateTable(
                name: "subjects",
                columns: table => new
                {
                    id = table.Column<Guid>(
                        type: "uuid",
                        nullable: false,
                        defaultValueSql: "uuidv7()"
                    ),
                    code = table.Column<string>(
                        type: "character varying(10)",
                        maxLength: 10,
                        nullable: false
                    ),
                    color = table.Column<string>(
                        type: "character varying(7)",
                        maxLength: 7,
                        nullable: false
                    ),
                    created_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false,
                        defaultValueSql: "NOW() AT TIME ZONE 'UTC'"
                    ),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    description = table.Column<string>(
                        type: "character varying(500)",
                        maxLength: 500,
                        nullable: true
                    ),
                    is_archived = table.Column<bool>(type: "boolean", nullable: false),
                    last_modified_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true,
                        defaultValueSql: "NOW() AT TIME ZONE 'UTC'"
                    ),
                    last_modified_by = table.Column<Guid>(type: "uuid", nullable: true),
                    name = table.Column<string>(
                        type: "character varying(120)",
                        maxLength: 120,
                        nullable: false
                    ),
                    order = table.Column<int>(type: "integer", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_subjects", x => x.id);
                }
            );

            migrationBuilder.CreateIndex(
                name: "ix_groups_level_id",
                table: "groups",
                column: "level_id"
            );

            migrationBuilder.CreateIndex(
                name: "ix_lesson_types_organization_id_code",
                table: "lesson_types",
                columns: new[] { "organization_id", "code" },
                unique: true,
                filter: "is_archived = false"
            );

            migrationBuilder.CreateIndex(
                name: "ix_lesson_types_organization_id_is_archived",
                table: "lesson_types",
                columns: new[] { "organization_id", "is_archived" }
            );

            migrationBuilder.CreateIndex(
                name: "ix_lesson_types_organization_id_name",
                table: "lesson_types",
                columns: new[] { "organization_id", "name" },
                unique: true,
                filter: "is_archived = false"
            );

            migrationBuilder.CreateIndex(
                name: "ix_levels_organization_id_code",
                table: "levels",
                columns: new[] { "organization_id", "code" },
                unique: true,
                filter: "is_deleted = false"
            );

            migrationBuilder.CreateIndex(
                name: "ix_levels_organization_id_is_active",
                table: "levels",
                columns: new[] { "organization_id", "is_active" }
            );

            migrationBuilder.CreateIndex(
                name: "ix_levels_organization_id_sort_order",
                table: "levels",
                columns: new[] { "organization_id", "sort_order" },
                unique: true,
                filter: "is_deleted = false"
            );

            migrationBuilder.CreateIndex(
                name: "ix_subjects_organization_id_code",
                table: "subjects",
                columns: new[] { "organization_id", "code" },
                unique: true,
                filter: "is_archived = false"
            );

            migrationBuilder.CreateIndex(
                name: "ix_subjects_organization_id_is_archived",
                table: "subjects",
                columns: new[] { "organization_id", "is_archived" }
            );

            migrationBuilder.CreateIndex(
                name: "ix_subjects_organization_id_name",
                table: "subjects",
                columns: new[] { "organization_id", "name" },
                unique: true,
                filter: "is_archived = false"
            );

            migrationBuilder.AddForeignKey(
                name: "fk_groups_levels_level_id",
                table: "groups",
                column: "level_id",
                principalTable: "levels",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict
            );
        }
    }
}
