using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edvantix.Persona.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MigratetoGuid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "profile_id",
                table: "profile_contact",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(20,0)");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "profile_contact",
                type: "uuid",
                nullable: false,
                defaultValueSql: "uuidv7()",
                oldClrType: typeof(decimal),
                oldType: "numeric(20,0)",
                oldComment: "Идентификатор");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "profile",
                type: "uuid",
                nullable: false,
                defaultValueSql: "uuidv7()",
                oldClrType: typeof(decimal),
                oldType: "numeric(20,0)",
                oldComment: "Идентификатор");

            migrationBuilder.AlterColumn<Guid>(
                name: "profile_id",
                table: "full_name",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(20,0)");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "full_name",
                type: "uuid",
                nullable: false,
                defaultValueSql: "uuidv7()",
                oldClrType: typeof(decimal),
                oldType: "numeric(20,0)",
                oldComment: "Идентификатор");

            migrationBuilder.AlterColumn<Guid>(
                name: "profile_id",
                table: "employment_history",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(20,0)");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "employment_history",
                type: "uuid",
                nullable: false,
                defaultValueSql: "uuidv7()",
                oldClrType: typeof(decimal),
                oldType: "numeric(20,0)",
                oldComment: "Идентификатор");

            migrationBuilder.AlterColumn<Guid>(
                name: "profile_id",
                table: "education",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(20,0)");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "education",
                type: "uuid",
                nullable: false,
                defaultValueSql: "uuidv7()",
                oldClrType: typeof(decimal),
                oldType: "numeric(20,0)",
                oldComment: "Идентификатор");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "profile_id",
                table: "profile_contact",
                type: "numeric(20,0)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<decimal>(
                name: "id",
                table: "profile_contact",
                type: "numeric(20,0)",
                nullable: false,
                comment: "Идентификатор",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "uuidv7()");

            migrationBuilder.AlterColumn<decimal>(
                name: "id",
                table: "profile",
                type: "numeric(20,0)",
                nullable: false,
                comment: "Идентификатор",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "uuidv7()");

            migrationBuilder.AlterColumn<decimal>(
                name: "profile_id",
                table: "full_name",
                type: "numeric(20,0)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<decimal>(
                name: "id",
                table: "full_name",
                type: "numeric(20,0)",
                nullable: false,
                comment: "Идентификатор",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "uuidv7()");

            migrationBuilder.AlterColumn<decimal>(
                name: "profile_id",
                table: "employment_history",
                type: "numeric(20,0)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<decimal>(
                name: "id",
                table: "employment_history",
                type: "numeric(20,0)",
                nullable: false,
                comment: "Идентификатор",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "uuidv7()");

            migrationBuilder.AlterColumn<decimal>(
                name: "profile_id",
                table: "education",
                type: "numeric(20,0)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<decimal>(
                name: "id",
                table: "education",
                type: "numeric(20,0)",
                nullable: false,
                comment: "Идентификатор",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "uuidv7()");
        }
    }
}
