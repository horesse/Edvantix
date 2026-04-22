using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edvantix.Organizational.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Addinvitesystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "invitations",
                columns: table => new
                {
                    id = table.Column<Guid>(
                        type: "uuid",
                        nullable: false,
                        defaultValueSql: "uuidv7()"
                    ),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    inviter_profile_id = table.Column<Guid>(type: "uuid", nullable: false),
                    role_id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<string>(
                        type: "character varying(20)",
                        maxLength: 20,
                        nullable: false
                    ),
                    status = table.Column<string>(
                        type: "character varying(20)",
                        maxLength: 20,
                        nullable: false
                    ),
                    token_hash = table.Column<string>(
                        type: "character varying(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    email = table.Column<string>(
                        type: "character varying(50)",
                        maxLength: 50,
                        nullable: true
                    ),
                    invitee_login = table.Column<string>(
                        type: "character varying(50)",
                        maxLength: 50,
                        nullable: true
                    ),
                    invitee_profile_id = table.Column<Guid>(type: "uuid", nullable: true),
                    invitee_account_id = table.Column<Guid>(type: "uuid", nullable: true),
                    expires_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    created_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    accepted_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    accepted_by_profile_id = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(
                        type: "boolean",
                        nullable: false,
                        comment: "Признак удаленной записи"
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_invitations", x => x.id);
                }
            );

            migrationBuilder.CreateIndex(
                name: "ix_invitations_organization_id_status",
                table: "invitations",
                columns: new[] { "organization_id", "status" }
            );

            migrationBuilder.CreateIndex(
                name: "ix_invitations_token_hash",
                table: "invitations",
                column: "token_hash",
                unique: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "invitations");
        }
    }
}
