using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edvantix.Organizational.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceIsArchivedWithIsDeleted : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(name: "ix_subjects_organization_id_code", table: "subjects");

            migrationBuilder.DropIndex(
                name: "ix_subjects_organization_id_is_archived",
                table: "subjects"
            );

            migrationBuilder.DropIndex(name: "ix_subjects_organization_id_name", table: "subjects");

            migrationBuilder.DropIndex(
                name: "ix_student_tags_org_name_active",
                table: "student_tags"
            );

            migrationBuilder.DropIndex(
                name: "ix_student_statuses_org_code_active",
                table: "student_statuses"
            );

            migrationBuilder.DropIndex(
                name: "ix_student_statuses_org_name_active",
                table: "student_statuses"
            );

            migrationBuilder.DropIndex(name: "ix_rooms_org_name_active", table: "rooms");

            migrationBuilder.DropIndex(
                name: "ix_payment_methods_org_code_active",
                table: "payment_methods"
            );

            migrationBuilder.DropIndex(
                name: "ix_payment_methods_org_name_active",
                table: "payment_methods"
            );

            migrationBuilder.DropIndex(
                name: "ix_lesson_types_organization_id_code",
                table: "lesson_types"
            );

            migrationBuilder.DropIndex(
                name: "ix_lesson_types_organization_id_is_archived",
                table: "lesson_types"
            );

            migrationBuilder.DropIndex(
                name: "ix_lesson_types_organization_id_name",
                table: "lesson_types"
            );

            migrationBuilder.DropIndex(
                name: "ix_lead_sources_org_name_active",
                table: "lead_sources"
            );

            migrationBuilder.DropColumn(name: "is_archived", table: "subjects");

            migrationBuilder.DropColumn(name: "is_archived", table: "student_tags");

            migrationBuilder.DropColumn(name: "is_archived", table: "student_statuses");

            migrationBuilder.DropColumn(name: "is_archived", table: "rooms");

            migrationBuilder.DropColumn(name: "is_archived", table: "payment_methods");

            migrationBuilder.DropColumn(name: "is_archived", table: "lesson_types");

            migrationBuilder.DropColumn(name: "is_archived", table: "lead_sources");

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "subjects",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                comment: "Признак удаленной записи"
            );

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "student_tags",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                comment: "Признак удаленной записи"
            );

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "student_statuses",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                comment: "Признак удаленной записи"
            );

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "rooms",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                comment: "Признак удаленной записи"
            );

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "payment_methods",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                comment: "Признак удаленной записи"
            );

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "lesson_types",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                comment: "Признак удаленной записи"
            );

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "lead_sources",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                comment: "Признак удаленной записи"
            );

            migrationBuilder.CreateIndex(
                name: "ix_subjects_organization_id_code",
                table: "subjects",
                columns: new[] { "organization_id", "code" },
                unique: true,
                filter: "is_deleted = false"
            );

            migrationBuilder.CreateIndex(
                name: "ix_subjects_organization_id_name",
                table: "subjects",
                columns: new[] { "organization_id", "name" },
                unique: true,
                filter: "is_deleted = false"
            );

            migrationBuilder.CreateIndex(
                name: "ix_student_tags_org_name_active",
                table: "student_tags",
                columns: new[] { "organization_id", "name" },
                unique: true,
                filter: "is_deleted = false"
            );

            migrationBuilder.CreateIndex(
                name: "ix_student_statuses_org_code_active",
                table: "student_statuses",
                columns: new[] { "organization_id", "code" },
                unique: true,
                filter: "is_deleted = false"
            );

            migrationBuilder.CreateIndex(
                name: "ix_student_statuses_org_name_active",
                table: "student_statuses",
                columns: new[] { "organization_id", "name" },
                unique: true,
                filter: "is_deleted = false"
            );

            migrationBuilder.CreateIndex(
                name: "ix_rooms_org_name_active",
                table: "rooms",
                columns: new[] { "organization_id", "name" },
                unique: true,
                filter: "is_deleted = false"
            );

            migrationBuilder.CreateIndex(
                name: "ix_payment_methods_org_code_active",
                table: "payment_methods",
                columns: new[] { "organization_id", "code" },
                unique: true,
                filter: "is_deleted = false"
            );

            migrationBuilder.CreateIndex(
                name: "ix_payment_methods_org_name_active",
                table: "payment_methods",
                columns: new[] { "organization_id", "name" },
                unique: true,
                filter: "is_deleted = false"
            );

            migrationBuilder.CreateIndex(
                name: "ix_lesson_types_organization_id_code",
                table: "lesson_types",
                columns: new[] { "organization_id", "code" },
                unique: true,
                filter: "is_deleted = false"
            );

            migrationBuilder.CreateIndex(
                name: "ix_lesson_types_organization_id_name",
                table: "lesson_types",
                columns: new[] { "organization_id", "name" },
                unique: true,
                filter: "is_deleted = false"
            );

            migrationBuilder.CreateIndex(
                name: "ix_lead_sources_org_name_active",
                table: "lead_sources",
                columns: new[] { "organization_id", "name" },
                unique: true,
                filter: "is_deleted = false"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(name: "ix_subjects_organization_id_code", table: "subjects");

            migrationBuilder.DropIndex(name: "ix_subjects_organization_id_name", table: "subjects");

            migrationBuilder.DropIndex(
                name: "ix_student_tags_org_name_active",
                table: "student_tags"
            );

            migrationBuilder.DropIndex(
                name: "ix_student_statuses_org_code_active",
                table: "student_statuses"
            );

            migrationBuilder.DropIndex(
                name: "ix_student_statuses_org_name_active",
                table: "student_statuses"
            );

            migrationBuilder.DropIndex(name: "ix_rooms_org_name_active", table: "rooms");

            migrationBuilder.DropIndex(
                name: "ix_payment_methods_org_code_active",
                table: "payment_methods"
            );

            migrationBuilder.DropIndex(
                name: "ix_payment_methods_org_name_active",
                table: "payment_methods"
            );

            migrationBuilder.DropIndex(
                name: "ix_lesson_types_organization_id_code",
                table: "lesson_types"
            );

            migrationBuilder.DropIndex(
                name: "ix_lesson_types_organization_id_name",
                table: "lesson_types"
            );

            migrationBuilder.DropIndex(
                name: "ix_lead_sources_org_name_active",
                table: "lead_sources"
            );

            migrationBuilder.DropColumn(name: "is_deleted", table: "subjects");

            migrationBuilder.DropColumn(name: "is_deleted", table: "student_tags");

            migrationBuilder.DropColumn(name: "is_deleted", table: "student_statuses");

            migrationBuilder.DropColumn(name: "is_deleted", table: "rooms");

            migrationBuilder.DropColumn(name: "is_deleted", table: "payment_methods");

            migrationBuilder.DropColumn(name: "is_deleted", table: "lesson_types");

            migrationBuilder.DropColumn(name: "is_deleted", table: "lead_sources");

            migrationBuilder.AddColumn<bool>(
                name: "is_archived",
                table: "subjects",
                type: "boolean",
                nullable: false,
                defaultValue: false
            );

            migrationBuilder.AddColumn<bool>(
                name: "is_archived",
                table: "student_tags",
                type: "boolean",
                nullable: false,
                defaultValue: false
            );

            migrationBuilder.AddColumn<bool>(
                name: "is_archived",
                table: "student_statuses",
                type: "boolean",
                nullable: false,
                defaultValue: false
            );

            migrationBuilder.AddColumn<bool>(
                name: "is_archived",
                table: "rooms",
                type: "boolean",
                nullable: false,
                defaultValue: false
            );

            migrationBuilder.AddColumn<bool>(
                name: "is_archived",
                table: "payment_methods",
                type: "boolean",
                nullable: false,
                defaultValue: false
            );

            migrationBuilder.AddColumn<bool>(
                name: "is_archived",
                table: "lesson_types",
                type: "boolean",
                nullable: false,
                defaultValue: false
            );

            migrationBuilder.AddColumn<bool>(
                name: "is_archived",
                table: "lead_sources",
                type: "boolean",
                nullable: false,
                defaultValue: false
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

            migrationBuilder.CreateIndex(
                name: "ix_student_tags_org_name_active",
                table: "student_tags",
                columns: new[] { "organization_id", "name" },
                unique: true,
                filter: "is_archived = false"
            );

            migrationBuilder.CreateIndex(
                name: "ix_student_statuses_org_code_active",
                table: "student_statuses",
                columns: new[] { "organization_id", "code" },
                unique: true,
                filter: "is_archived = false"
            );

            migrationBuilder.CreateIndex(
                name: "ix_student_statuses_org_name_active",
                table: "student_statuses",
                columns: new[] { "organization_id", "name" },
                unique: true,
                filter: "is_archived = false"
            );

            migrationBuilder.CreateIndex(
                name: "ix_rooms_org_name_active",
                table: "rooms",
                columns: new[] { "organization_id", "name" },
                unique: true,
                filter: "is_archived = false"
            );

            migrationBuilder.CreateIndex(
                name: "ix_payment_methods_org_code_active",
                table: "payment_methods",
                columns: new[] { "organization_id", "code" },
                unique: true,
                filter: "is_archived = false"
            );

            migrationBuilder.CreateIndex(
                name: "ix_payment_methods_org_name_active",
                table: "payment_methods",
                columns: new[] { "organization_id", "name" },
                unique: true,
                filter: "is_archived = false"
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
                name: "ix_lead_sources_org_name_active",
                table: "lead_sources",
                columns: new[] { "organization_id", "name" },
                unique: true,
                filter: "is_archived = false"
            );
        }
    }
}
