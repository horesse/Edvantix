using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edvantix.Schedule.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "group_schedules",
                columns: table => new
                {
                    id = table.Column<Guid>(
                        type: "uuid",
                        nullable: false,
                        defaultValueSql: "uuidv7()"
                    ),
                    group_id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    recurrence = table.Column<string>(
                        type: "character varying(20)",
                        maxLength: 20,
                        nullable: false
                    ),
                    biweekly_parity = table.Column<int>(type: "integer", nullable: true),
                    lesson_duration_minutes = table.Column<short>(
                        type: "smallint",
                        nullable: false
                    ),
                    start_date = table.Column<DateOnly>(type: "date", nullable: false),
                    end_mode = table.Column<string>(
                        type: "character varying(20)",
                        maxLength: 20,
                        nullable: false
                    ),
                    end_date = table.Column<DateOnly>(type: "date", nullable: true),
                    lesson_count = table.Column<short>(type: "smallint", nullable: true),
                    skip_holidays = table.Column<bool>(type: "boolean", nullable: false),
                    notify_students = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    updated_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_group_schedules", x => x.id);
                }
            );

            migrationBuilder.CreateTable(
                name: "holidays",
                columns: table => new
                {
                    id = table.Column<Guid>(
                        type: "uuid",
                        nullable: false,
                        defaultValueSql: "uuidv7()"
                    ),
                    country_code = table.Column<string>(
                        type: "character varying(3)",
                        maxLength: 3,
                        nullable: false
                    ),
                    date = table.Column<DateOnly>(type: "date", nullable: false),
                    name = table.Column<string>(
                        type: "character varying(50)",
                        maxLength: 50,
                        nullable: false
                    ),
                    is_recurring_annually = table.Column<bool>(type: "boolean", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_holidays", x => x.id);
                }
            );

            migrationBuilder.CreateTable(
                name: "lesson_occurrences",
                columns: table => new
                {
                    id = table.Column<Guid>(
                        type: "uuid",
                        nullable: false,
                        defaultValueSql: "uuidv7()"
                    ),
                    schedule_id = table.Column<Guid>(type: "uuid", nullable: false),
                    group_id = table.Column<Guid>(type: "uuid", nullable: false),
                    lesson_date = table.Column<DateOnly>(type: "date", nullable: false),
                    start_minutes = table.Column<int>(type: "integer", nullable: false),
                    duration_minutes = table.Column<short>(type: "smallint", nullable: false),
                    status = table.Column<string>(
                        type: "character varying(20)",
                        maxLength: 20,
                        nullable: false
                    ),
                    skip_reason = table.Column<string>(
                        type: "character varying(100)",
                        maxLength: 100,
                        nullable: true
                    ),
                    lesson_ref_id = table.Column<Guid>(type: "uuid", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_lesson_occurrences", x => x.id);
                }
            );

            migrationBuilder.CreateTable(
                name: "schedule_exceptions",
                columns: table => new
                {
                    id = table.Column<Guid>(
                        type: "uuid",
                        nullable: false,
                        defaultValueSql: "uuidv7()"
                    ),
                    schedule_id = table.Column<Guid>(type: "uuid", nullable: false),
                    exception_date = table.Column<DateOnly>(type: "date", nullable: false),
                    reason = table.Column<string>(
                        type: "character varying(100)",
                        maxLength: 100,
                        nullable: true
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_schedule_exceptions", x => x.id);
                    table.ForeignKey(
                        name: "fk_schedule_exceptions_group_schedules_schedule_id",
                        column: x => x.schedule_id,
                        principalTable: "group_schedules",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "schedule_slots",
                columns: table => new
                {
                    id = table.Column<Guid>(
                        type: "uuid",
                        nullable: false,
                        defaultValueSql: "uuidv7()"
                    ),
                    schedule_id = table.Column<Guid>(type: "uuid", nullable: false),
                    weekday = table.Column<int>(type: "integer", nullable: false),
                    start_minutes = table.Column<int>(type: "integer", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_schedule_slots", x => x.id);
                    table.ForeignKey(
                        name: "fk_schedule_slots_group_schedules_schedule_id",
                        column: x => x.schedule_id,
                        principalTable: "group_schedules",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "ix_group_schedules_group_id",
                table: "group_schedules",
                column: "group_id",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "ix_group_schedules_organization_id",
                table: "group_schedules",
                column: "organization_id"
            );

            migrationBuilder.CreateIndex(
                name: "ix_holidays_country_code",
                table: "holidays",
                column: "country_code"
            );

            migrationBuilder.CreateIndex(
                name: "ix_holidays_country_code_date",
                table: "holidays",
                columns: new[] { "country_code", "date" },
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "ix_lesson_occurrences_group_id_lesson_date",
                table: "lesson_occurrences",
                columns: new[] { "group_id", "lesson_date" }
            );

            migrationBuilder.CreateIndex(
                name: "ix_lesson_occurrences_schedule_id_status",
                table: "lesson_occurrences",
                columns: new[] { "schedule_id", "status" }
            );

            migrationBuilder.CreateIndex(
                name: "ix_schedule_exceptions_schedule_id_exception_date",
                table: "schedule_exceptions",
                columns: new[] { "schedule_id", "exception_date" },
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "ix_schedule_slots_schedule_id_weekday_start_minutes",
                table: "schedule_slots",
                columns: new[] { "schedule_id", "weekday", "start_minutes" },
                unique: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "holidays");

            migrationBuilder.DropTable(name: "lesson_occurrences");

            migrationBuilder.DropTable(name: "schedule_exceptions");

            migrationBuilder.DropTable(name: "schedule_slots");

            migrationBuilder.DropTable(name: "group_schedules");
        }
    }
}
