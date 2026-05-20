using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edvantix.Organizational.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveGroupsAndLevels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "group_members");

            migrationBuilder.DropTable(name: "groups");

            migrationBuilder.DropTable(name: "levels");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
                name: "groups",
                columns: table => new
                {
                    id = table.Column<Guid>(
                        type: "uuid",
                        nullable: false,
                        defaultValueSql: "uuidv7()"
                    ),
                    level_id = table.Column<Guid>(type: "uuid", nullable: false),
                    capacity = table.Column<int>(type: "integer", nullable: false),
                    code = table.Column<string>(
                        type: "character varying(32)",
                        maxLength: 32,
                        nullable: false
                    ),
                    course_id = table.Column<Guid>(type: "uuid", nullable: false),
                    description = table.Column<string>(
                        type: "character varying(255)",
                        maxLength: 255,
                        nullable: false
                    ),
                    end_date = table.Column<DateOnly>(type: "date", nullable: false),
                    format = table.Column<string>(
                        type: "character varying(20)",
                        maxLength: 20,
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
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    platform = table.Column<string>(
                        type: "character varying(20)",
                        maxLength: 20,
                        nullable: true
                    ),
                    room_id = table.Column<Guid>(type: "uuid", nullable: true),
                    start_date = table.Column<DateOnly>(type: "date", nullable: false),
                    status = table.Column<string>(
                        type: "character varying(20)",
                        maxLength: 20,
                        nullable: false
                    ),
                    teacher_member_id = table.Column<Guid>(type: "uuid", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_groups", x => x.id);
                    table.ForeignKey(
                        name: "fk_groups_levels_level_id",
                        column: x => x.level_id,
                        principalTable: "levels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "group_members",
                columns: table => new
                {
                    id = table.Column<Guid>(
                        type: "uuid",
                        nullable: false,
                        defaultValueSql: "uuidv7()"
                    ),
                    exit_reason = table.Column<string>(
                        type: "character varying(255)",
                        maxLength: 255,
                        nullable: true
                    ),
                    exited_at = table.Column<DateOnly>(type: "date", nullable: true),
                    group_id = table.Column<Guid>(type: "uuid", nullable: false),
                    joined_at = table.Column<DateOnly>(type: "date", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    profile_id = table.Column<Guid>(type: "uuid", nullable: false),
                    role = table.Column<string>(
                        type: "character varying(20)",
                        maxLength: 20,
                        nullable: false
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_group_members", x => x.id);
                    table.ForeignKey(
                        name: "fk_group_members_groups_group_id",
                        column: x => x.group_id,
                        principalTable: "groups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "ix_group_members_group_id",
                table: "group_members",
                column: "group_id"
            );

            migrationBuilder.CreateIndex(
                name: "ix_groups_course_id",
                table: "groups",
                column: "course_id"
            );

            migrationBuilder.CreateIndex(
                name: "ix_groups_level_id",
                table: "groups",
                column: "level_id"
            );

            migrationBuilder.CreateIndex(
                name: "ix_groups_organization_id_code",
                table: "groups",
                columns: new[] { "organization_id", "code" },
                unique: true,
                filter: "is_deleted = false"
            );

            migrationBuilder.CreateIndex(
                name: "ix_groups_organization_id_level_id_status",
                table: "groups",
                columns: new[] { "organization_id", "level_id", "status" }
            );

            migrationBuilder.CreateIndex(
                name: "ix_groups_teacher_member_id",
                table: "groups",
                column: "teacher_member_id"
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
        }
    }
}
