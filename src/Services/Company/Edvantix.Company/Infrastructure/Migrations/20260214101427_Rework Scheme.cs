using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Edvantix.Company.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ReworkScheme : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "member");

            migrationBuilder.DropTable(
                name: "usage");

            migrationBuilder.DropTable(
                name: "subscription");

            migrationBuilder.CreateTable(
                name: "group",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "Идентификатор")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    organization_id = table.Column<long>(type: "bigint", nullable: false),
                    name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    description = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, comment: "Признак удаленной записи")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_group", x => x.id);
                    table.ForeignKey(
                        name: "fk_group_organization_organization_id",
                        column: x => x.organization_id,
                        principalTable: "organization",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "organization_member",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, comment: "Идентификатор"),
                    organization_id = table.Column<long>(type: "bigint", nullable: false),
                    profile_id = table.Column<long>(type: "bigint", nullable: false),
                    role = table.Column<int>(type: "integer", nullable: false),
                    joined_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, comment: "Признак удаленной записи")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_organization_member", x => x.id);
                    table.ForeignKey(
                        name: "fk_organization_member_organization_organization_id",
                        column: x => x.organization_id,
                        principalTable: "organization",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "group_member",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, comment: "Идентификатор"),
                    group_id = table.Column<long>(type: "bigint", nullable: false),
                    profile_id = table.Column<long>(type: "bigint", nullable: false),
                    role = table.Column<int>(type: "integer", nullable: false),
                    joined_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, comment: "Признак удаленной записи")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_group_member", x => x.id);
                    table.ForeignKey(
                        name: "fk_group_member_group_group_id",
                        column: x => x.group_id,
                        principalTable: "group",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_group_name",
                table: "group",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "ix_group_organization_id",
                table: "group",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "ix_group_member_group_id",
                table: "group_member",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "ix_group_member_group_id_profile_id_is_deleted",
                table: "group_member",
                columns: new[] { "group_id", "profile_id", "is_deleted" });

            migrationBuilder.CreateIndex(
                name: "ix_group_member_profile_id",
                table: "group_member",
                column: "profile_id");

            migrationBuilder.CreateIndex(
                name: "ix_organization_member_organization_id",
                table: "organization_member",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "ix_organization_member_organization_id_profile_id_is_deleted",
                table: "organization_member",
                columns: new[] { "organization_id", "profile_id", "is_deleted" });

            migrationBuilder.CreateIndex(
                name: "ix_organization_member_profile_id",
                table: "organization_member",
                column: "profile_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "group_member");

            migrationBuilder.DropTable(
                name: "organization_member");

            migrationBuilder.DropTable(
                name: "group");

            migrationBuilder.CreateTable(
                name: "member",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, comment: "Идентификатор"),
                    organization_id = table.Column<long>(type: "bigint", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, comment: "Признак удаленной записи"),
                    person_id = table.Column<Guid>(type: "uuid", nullable: false),
                    position = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_member", x => x.id);
                    table.ForeignKey(
                        name: "fk_member_organization_organization_id",
                        column: x => x.organization_id,
                        principalTable: "organization",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "subscription",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "Идентификатор")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    organization_id = table.Column<long>(type: "bigint", nullable: false),
                    date_end = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    date_start = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    subscription_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_subscription", x => x.id);
                    table.ForeignKey(
                        name: "fk_subscription_organization_organization_id",
                        column: x => x.organization_id,
                        principalTable: "organization",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "usage",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "Идентификатор")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    organization_id = table.Column<long>(type: "bigint", nullable: false),
                    limit_id = table.Column<long>(type: "bigint", nullable: false),
                    value = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_usage", x => x.id);
                    table.ForeignKey(
                        name: "fk_usage_organization_organization_id",
                        column: x => x.organization_id,
                        principalTable: "organization",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_usage_subscription_organization_id",
                        column: x => x.organization_id,
                        principalTable: "subscription",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_member_is_deleted",
                table: "member",
                column: "is_deleted");

            migrationBuilder.CreateIndex(
                name: "ix_member_organization_id",
                table: "member",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "ix_member_organization_id_person_id_is_deleted",
                table: "member",
                columns: new[] { "organization_id", "person_id", "is_deleted" });

            migrationBuilder.CreateIndex(
                name: "ix_member_person_id",
                table: "member",
                column: "person_id");

            migrationBuilder.CreateIndex(
                name: "ix_subscription_date_end",
                table: "subscription",
                column: "date_end");

            migrationBuilder.CreateIndex(
                name: "ix_subscription_organization_id",
                table: "subscription",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "ix_subscription_organization_id_date_start_date_end",
                table: "subscription",
                columns: new[] { "organization_id", "date_start", "date_end" });

            migrationBuilder.CreateIndex(
                name: "ix_subscription_subscription_id",
                table: "subscription",
                column: "subscription_id");

            migrationBuilder.CreateIndex(
                name: "ix_usage_limit_id",
                table: "usage",
                column: "limit_id");

            migrationBuilder.CreateIndex(
                name: "ix_usage_organization_id",
                table: "usage",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "ix_usage_organization_id_limit_id",
                table: "usage",
                columns: new[] { "organization_id", "limit_id" });
        }
    }
}
