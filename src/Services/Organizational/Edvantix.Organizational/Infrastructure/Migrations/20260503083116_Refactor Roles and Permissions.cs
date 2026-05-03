using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edvantix.Organizational.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RefactorRolesandPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_permissions_service_code_feature_code_code",
                table: "permissions"
            );

            migrationBuilder.DropColumn(name: "feature_name", table: "permissions");

            migrationBuilder.DropColumn(name: "service_code", table: "permissions");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "permissions",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200
            );

            migrationBuilder.AlterColumn<string>(
                name: "feature_code",
                table: "permissions",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200
            );

            migrationBuilder.AlterColumn<string>(
                name: "code",
                table: "permissions",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200
            );

            migrationBuilder.CreateTable(
                name: "features",
                columns: table => new
                {
                    id = table.Column<Guid>(
                        type: "uuid",
                        nullable: false,
                        defaultValueSql: "uuidv7()"
                    ),
                    service_code = table.Column<string>(
                        type: "character varying(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    code = table.Column<string>(
                        type: "character varying(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    name = table.Column<string>(
                        type: "character varying(255)",
                        maxLength: 255,
                        nullable: false
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_features", x => x.id);
                    table.UniqueConstraint("ak_features_code", x => x.code);
                }
            );

            migrationBuilder.CreateIndex(
                name: "ix_permissions_feature_code_code",
                table: "permissions",
                columns: new[] { "feature_code", "code" },
                unique: true
            );

            migrationBuilder.AddForeignKey(
                name: "fk_permissions_features_feature_code",
                table: "permissions",
                column: "feature_code",
                principalTable: "features",
                principalColumn: "code",
                onDelete: ReferentialAction.Restrict
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_permissions_features_feature_code",
                table: "permissions"
            );

            migrationBuilder.DropTable(name: "features");

            migrationBuilder.DropIndex(
                name: "ix_permissions_feature_code_code",
                table: "permissions"
            );

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "permissions",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255
            );

            migrationBuilder.AlterColumn<string>(
                name: "feature_code",
                table: "permissions",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100
            );

            migrationBuilder.AlterColumn<string>(
                name: "code",
                table: "permissions",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100
            );

            migrationBuilder.AddColumn<string>(
                name: "feature_name",
                table: "permissions",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: ""
            );

            migrationBuilder.AddColumn<string>(
                name: "service_code",
                table: "permissions",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: ""
            );

            migrationBuilder.CreateIndex(
                name: "ix_permissions_service_code_feature_code_code",
                table: "permissions",
                columns: new[] { "service_code", "feature_code", "code" },
                unique: true
            );
        }
    }
}
