using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edvantix.Notification.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameAccountIdToProfileIdInInAppNotifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "account_id",
                table: "in_app_notifications",
                newName: "profile_id"
            );

            migrationBuilder.RenameIndex(
                name: "ix_in_app_notifications_account_id_is_read",
                table: "in_app_notifications",
                newName: "ix_in_app_notifications_profile_id_is_read"
            );

            migrationBuilder.RenameIndex(
                name: "ix_in_app_notifications_account_id_created_at",
                table: "in_app_notifications",
                newName: "ix_in_app_notifications_profile_id_created_at"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "profile_id",
                table: "in_app_notifications",
                newName: "account_id"
            );

            migrationBuilder.RenameIndex(
                name: "ix_in_app_notifications_profile_id_is_read",
                table: "in_app_notifications",
                newName: "ix_in_app_notifications_account_id_is_read"
            );

            migrationBuilder.RenameIndex(
                name: "ix_in_app_notifications_profile_id_created_at",
                table: "in_app_notifications",
                newName: "ix_in_app_notifications_account_id_created_at"
            );
        }
    }
}
