using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edvantix.Organizational.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveGroupMemberHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "group_membership_histories");

            migrationBuilder.AlterColumn<bool>(
                name: "is_deleted",
                table: "group_members",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldComment: "Признак удаленной записи"
            );

            migrationBuilder.AddColumn<string>(
                name: "exit_reason",
                table: "group_members",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true
            );

            migrationBuilder.AddColumn<DateOnly>(
                name: "exited_at",
                table: "group_members",
                type: "date",
                nullable: true
            );

            migrationBuilder.AddColumn<DateOnly>(
                name: "joined_at",
                table: "group_members",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1)
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "exit_reason", table: "group_members");

            migrationBuilder.DropColumn(name: "exited_at", table: "group_members");

            migrationBuilder.DropColumn(name: "joined_at", table: "group_members");

            migrationBuilder.AlterColumn<bool>(
                name: "is_deleted",
                table: "group_members",
                type: "boolean",
                nullable: false,
                comment: "Признак удаленной записи",
                oldClrType: typeof(bool),
                oldType: "boolean"
            );

            migrationBuilder.CreateTable(
                name: "group_membership_histories",
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
                    group_member_id = table.Column<Guid>(type: "uuid", nullable: false),
                    joined_at = table.Column<DateOnly>(type: "date", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_group_membership_histories", x => x.id);
                    table.ForeignKey(
                        name: "fk_group_membership_histories_group_members_group_member_id",
                        column: x => x.group_member_id,
                        principalTable: "group_members",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict
                    );
                },
                comment: "Иммутабельный журнал вступления и выхода участников из групп."
            );

            migrationBuilder.CreateIndex(
                name: "ix_group_membership_histories_group_member_id",
                table: "group_membership_histories",
                column: "group_member_id"
            );
        }
    }
}
