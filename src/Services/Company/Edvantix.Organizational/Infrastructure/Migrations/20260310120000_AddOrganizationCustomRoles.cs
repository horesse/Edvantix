using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edvantix.Organizational.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOrganizationCustomRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "organization_custom_role",
                columns: table => new
                {
                    id = table.Column<Guid>(
                        type: "uuid",
                        nullable: false,
                        defaultValueSql: "uuidv7()"
                    ),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(
                        type: "character varying(50)",
                        maxLength: 50,
                        nullable: false
                    ),
                    description = table.Column<string>(
                        type: "character varying(100)",
                        maxLength: 100,
                        nullable: true
                    ),
                    base_role = table.Column<short>(type: "smallint", nullable: false),
                    is_deleted = table.Column<bool>(
                        type: "boolean",
                        nullable: false,
                        comment: "Признак удаленной записи"
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_organization_custom_role", x => x.id);
                    table.ForeignKey(
                        name: "fk_organization_custom_role_organization_organization_id",
                        column: x => x.organization_id,
                        principalTable: "organization",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            // Индекс для поиска по организации (FK).
            migrationBuilder.CreateIndex(
                name: "ix_organization_custom_role_organization_id",
                table: "organization_custom_role",
                column: "organization_id"
            );

            // Уникальный частичный индекс: код роли уникален в рамках организации среди не удалённых записей.
            migrationBuilder.CreateIndex(
                name: "ix_organization_custom_role_organization_id_code",
                table: "organization_custom_role",
                columns: new[] { "organization_id", "code" },
                unique: true,
                filter: "NOT is_deleted"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "organization_custom_role");
        }
    }
}
