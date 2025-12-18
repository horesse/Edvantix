using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Edvantix.OrganizationManagement.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "organization",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "Идентификатор")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    name_latin = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    short_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    print_name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    description = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: true),
                    registration_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_organization", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "contact",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "Идентификатор")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    organization_id = table.Column<long>(type: "bigint", nullable: false),
                    type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    value = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_contact", x => x.id);
                    table.ForeignKey(
                        name: "fk_contact_organization_organization_id",
                        column: x => x.organization_id,
                        principalTable: "organization",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "member",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, comment: "Идентификатор"),
                    organization_id = table.Column<long>(type: "bigint", nullable: false),
                    person_id = table.Column<Guid>(type: "uuid", nullable: false),
                    position = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, comment: "Признак удаленной записи")
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
                    subscription_id = table.Column<long>(type: "bigint", nullable: false),
                    organization_id = table.Column<long>(type: "bigint", nullable: false),
                    date_start = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    date_end = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
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
                name: "ix_contact_organization_id",
                table: "contact",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "ix_contact_organization_id_type",
                table: "contact",
                columns: new[] { "organization_id", "type" });

            migrationBuilder.CreateIndex(
                name: "ix_contact_type",
                table: "contact",
                column: "type");

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
                name: "ix_organization_name",
                table: "organization",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "ix_organization_name_latin",
                table: "organization",
                column: "name_latin");

            migrationBuilder.CreateIndex(
                name: "ix_organization_registration_date",
                table: "organization",
                column: "registration_date");

            migrationBuilder.CreateIndex(
                name: "ix_organization_short_name",
                table: "organization",
                column: "short_name");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "contact");

            migrationBuilder.DropTable(
                name: "member");

            migrationBuilder.DropTable(
                name: "usage");

            migrationBuilder.DropTable(
                name: "subscription");

            migrationBuilder.DropTable(
                name: "organization");
        }
    }
}
