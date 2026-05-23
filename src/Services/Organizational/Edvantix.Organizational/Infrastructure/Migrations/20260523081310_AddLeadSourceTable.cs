using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edvantix.Organizational.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLeadSourceTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "lead_sources",
                columns: table => new
                {
                    id = table.Column<Guid>(
                        type: "uuid",
                        nullable: false,
                        defaultValueSql: "uuidv7()"
                    ),
                    channel = table.Column<string>(type: "text", nullable: false),
                    utm_tag = table.Column<string>(
                        type: "character varying(60)",
                        maxLength: 60,
                        nullable: true
                    ),
                    created_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    last_modified_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    row_version = table.Column<long>(type: "bigint", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(
                        type: "character varying(120)",
                        maxLength: 120,
                        nullable: false
                    ),
                    is_archived = table.Column<bool>(type: "boolean", nullable: false),
                    order = table.Column<int>(type: "integer", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    last_modified_by = table.Column<Guid>(type: "uuid", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_lead_sources", x => x.id);
                }
            );

            migrationBuilder.CreateIndex(
                name: "ix_lead_sources_org_id",
                table: "lead_sources",
                column: "organization_id"
            );

            migrationBuilder.CreateIndex(
                name: "ix_lead_sources_org_name_active",
                table: "lead_sources",
                columns: new[] { "organization_id", "name" },
                unique: true,
                filter: "is_archived = false"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "lead_sources");
        }
    }
}
