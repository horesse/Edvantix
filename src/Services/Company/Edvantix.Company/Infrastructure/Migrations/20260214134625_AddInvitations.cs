using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edvantix.Company.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInvitations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "invitation",
                columns: table => new
                {
                    id = table.Column<Guid>(
                        type: "uuid",
                        nullable: false,
                        comment: "Идентификатор"
                    ),
                    organization_id = table.Column<long>(type: "bigint", nullable: false),
                    invited_by_profile_id = table.Column<long>(type: "bigint", nullable: false),
                    invitee_profile_id = table.Column<long>(type: "bigint", nullable: true),
                    invitee_email = table.Column<string>(
                        type: "character varying(256)",
                        maxLength: 256,
                        nullable: true
                    ),
                    role = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    token = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    expires_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    responded_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_invitation", x => x.id);
                }
            );

            migrationBuilder.CreateIndex(
                name: "ix_invitation_invitee_profile_id_status",
                table: "invitation",
                columns: new[] { "invitee_profile_id", "status" }
            );

            migrationBuilder.CreateIndex(
                name: "ix_invitation_organization_id_invitee_email_status",
                table: "invitation",
                columns: new[] { "organization_id", "invitee_email", "status" }
            );

            migrationBuilder.CreateIndex(
                name: "ix_invitation_organization_id_status",
                table: "invitation",
                columns: new[] { "organization_id", "status" }
            );

            migrationBuilder.CreateIndex(
                name: "ix_invitation_token",
                table: "invitation",
                column: "token",
                unique: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "invitation");
        }
    }
}
