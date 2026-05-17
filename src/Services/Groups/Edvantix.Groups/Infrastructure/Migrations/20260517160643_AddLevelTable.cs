using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edvantix.Groups.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLevelTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(
                        type: "character varying(16)",
                        maxLength: 16,
                        nullable: false
                    ),
                    name = table.Column<string>(
                        type: "character varying(64)",
                        maxLength: 64,
                        nullable: false
                    ),
                    description = table.Column<string>(
                        type: "character varying(256)",
                        maxLength: 256,
                        nullable: true
                    ),
                    tone = table.Column<string>(
                        type: "character varying(20)",
                        maxLength: 20,
                        nullable: false
                    ),
                    sort_order = table.Column<short>(type: "smallint", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(
                        type: "boolean",
                        nullable: false,
                        comment: "Признак удаленной записи"
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_levels", x => x.id);
                }
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "levels");
        }
    }
}
