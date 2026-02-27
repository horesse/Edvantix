using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edvantix.Persona.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Rebase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_education_profile_profile_id",
                table: "education"
            );

            migrationBuilder.DropForeignKey(
                name: "fk_employment_history_profile_profile_id",
                table: "employment_history"
            );

            migrationBuilder.DropForeignKey(
                name: "fk_full_name_profile_profile_id",
                table: "full_name"
            );

            migrationBuilder.DropForeignKey(
                name: "fk_profile_contact_profile_profile_id",
                table: "profile_contact"
            );

            migrationBuilder.DropPrimaryKey(name: "pk_profile", table: "profile");

            migrationBuilder.RenameTable(name: "profile", newName: "profiles");

            migrationBuilder.RenameIndex(
                name: "ix_profile_account_id",
                table: "profiles",
                newName: "ix_profiles_account_id"
            );

            migrationBuilder.AddPrimaryKey(name: "pk_profiles", table: "profiles", column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_education_profiles_profile_id",
                table: "education",
                column: "profile_id",
                principalTable: "profiles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "fk_employment_history_profiles_profile_id",
                table: "employment_history",
                column: "profile_id",
                principalTable: "profiles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "fk_full_name_profiles_profile_id",
                table: "full_name",
                column: "profile_id",
                principalTable: "profiles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "fk_profile_contact_profiles_profile_id",
                table: "profile_contact",
                column: "profile_id",
                principalTable: "profiles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_education_profiles_profile_id",
                table: "education"
            );

            migrationBuilder.DropForeignKey(
                name: "fk_employment_history_profiles_profile_id",
                table: "employment_history"
            );

            migrationBuilder.DropForeignKey(
                name: "fk_full_name_profiles_profile_id",
                table: "full_name"
            );

            migrationBuilder.DropForeignKey(
                name: "fk_profile_contact_profiles_profile_id",
                table: "profile_contact"
            );

            migrationBuilder.DropPrimaryKey(name: "pk_profiles", table: "profiles");

            migrationBuilder.RenameTable(name: "profiles", newName: "profile");

            migrationBuilder.RenameIndex(
                name: "ix_profiles_account_id",
                table: "profile",
                newName: "ix_profile_account_id"
            );

            migrationBuilder.AddPrimaryKey(name: "pk_profile", table: "profile", column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_education_profile_profile_id",
                table: "education",
                column: "profile_id",
                principalTable: "profile",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "fk_employment_history_profile_profile_id",
                table: "employment_history",
                column: "profile_id",
                principalTable: "profile",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "fk_full_name_profile_profile_id",
                table: "full_name",
                column: "profile_id",
                principalTable: "profile",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "fk_profile_contact_profile_profile_id",
                table: "profile_contact",
                column: "profile_id",
                principalTable: "profile",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade
            );
        }
    }
}
