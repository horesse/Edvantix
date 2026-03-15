using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edvantix.Persona.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSkillsAndBio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "bio",
                table: "profiles",
                type: "character varying(600)",
                maxLength: 600,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "skills",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_skills", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "profile_skill",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    skill_id = table.Column<Guid>(type: "uuid", nullable: false),
                    profile_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_profile_skill", x => x.id);
                    table.ForeignKey(
                        name: "fk_profile_skill_profiles_profile_id",
                        column: x => x.profile_id,
                        principalTable: "profiles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_profile_skill_skills_skill_id",
                        column: x => x.skill_id,
                        principalTable: "skills",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_profile_skill_profile_id",
                table: "profile_skill",
                column: "profile_id");

            migrationBuilder.CreateIndex(
                name: "ix_profile_skill_profile_id_skill_id",
                table: "profile_skill",
                columns: new[] { "profile_id", "skill_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_profile_skill_skill_id",
                table: "profile_skill",
                column: "skill_id");

            migrationBuilder.CreateIndex(
                name: "ix_skills_name",
                table: "skills",
                column: "name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "profile_skill");

            migrationBuilder.DropTable(
                name: "skills");

            migrationBuilder.DropColumn(
                name: "bio",
                table: "profiles");
        }
    }
}
