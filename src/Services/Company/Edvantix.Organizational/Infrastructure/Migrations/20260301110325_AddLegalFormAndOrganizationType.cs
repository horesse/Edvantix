using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edvantix.Organizational.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLegalFormAndOrganizationType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "legal_form_id",
                table: "organization",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000")
            );

            migrationBuilder.AddColumn<int>(
                name: "organization_type",
                table: "organization",
                type: "integer",
                nullable: false,
                defaultValue: 0
            );

            migrationBuilder.CreateTable(
                name: "legal_form",
                columns: table => new
                {
                    id = table.Column<Guid>(
                        type: "uuid",
                        nullable: false,
                        defaultValueSql: "uuidv7()"
                    ),
                    name = table.Column<string>(
                        type: "character varying(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    short_name = table.Column<string>(
                        type: "character varying(20)",
                        maxLength: 20,
                        nullable: false
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_legal_form", x => x.id);
                }
            );

            migrationBuilder.CreateIndex(
                name: "ix_organization_legal_form_id",
                table: "organization",
                column: "legal_form_id"
            );

            migrationBuilder.CreateIndex(
                name: "ix_legal_form_short_name",
                table: "legal_form",
                column: "short_name",
                unique: true
            );

            migrationBuilder.AddForeignKey(
                name: "fk_organization_legal_form_legal_form_id",
                table: "organization",
                column: "legal_form_id",
                principalTable: "legal_form",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_organization_legal_form_legal_form_id",
                table: "organization"
            );

            migrationBuilder.DropTable(name: "legal_form");

            migrationBuilder.DropIndex(
                name: "ix_organization_legal_form_id",
                table: "organization"
            );

            migrationBuilder.DropColumn(name: "legal_form_id", table: "organization");

            migrationBuilder.DropColumn(name: "organization_type", table: "organization");
        }
    }
}
