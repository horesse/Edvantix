using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edvantix.Blog.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MigratetoGuid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "tag",
                type: "uuid",
                nullable: false,
                defaultValueSql: "uuidv7()",
                oldClrType: typeof(decimal),
                oldType: "numeric(20,0)",
                oldComment: "Идентификатор"
            );

            migrationBuilder.AlterColumn<Guid>(
                name: "tags_id",
                table: "post_tags",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(20,0)"
            );

            migrationBuilder.AlterColumn<Guid>(
                name: "post_id",
                table: "post_tags",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(20,0)"
            );

            migrationBuilder.AlterColumn<Guid>(
                name: "profile_id",
                table: "post_like",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(20,0)"
            );

            migrationBuilder.AlterColumn<Guid>(
                name: "post_id",
                table: "post_like",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(20,0)"
            );

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "post_like",
                type: "uuid",
                nullable: false,
                defaultValueSql: "uuidv7()",
                oldClrType: typeof(decimal),
                oldType: "numeric(20,0)",
                oldComment: "Идентификатор"
            );

            migrationBuilder.AlterColumn<Guid>(
                name: "post_id",
                table: "post_categories",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(20,0)"
            );

            migrationBuilder.AlterColumn<Guid>(
                name: "categories_id",
                table: "post_categories",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(20,0)"
            );

            migrationBuilder.AlterColumn<Guid>(
                name: "author_id",
                table: "post",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(20,0)"
            );

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "post",
                type: "uuid",
                nullable: false,
                defaultValueSql: "uuidv7()",
                oldClrType: typeof(decimal),
                oldType: "numeric(20,0)",
                oldComment: "Идентификатор"
            );

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "category",
                type: "uuid",
                nullable: false,
                defaultValueSql: "uuidv7()",
                oldClrType: typeof(decimal),
                oldType: "numeric(20,0)",
                oldComment: "Идентификатор"
            );

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "blog_subscription",
                type: "uuid",
                nullable: false,
                defaultValueSql: "uuidv7()",
                oldClrType: typeof(decimal),
                oldType: "numeric(20,0)",
                oldComment: "Идентификатор"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "id",
                table: "tag",
                type: "numeric(20,0)",
                nullable: false,
                comment: "Идентификатор",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "uuidv7()"
            );

            migrationBuilder.AlterColumn<decimal>(
                name: "tags_id",
                table: "post_tags",
                type: "numeric(20,0)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid"
            );

            migrationBuilder.AlterColumn<decimal>(
                name: "post_id",
                table: "post_tags",
                type: "numeric(20,0)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid"
            );

            migrationBuilder.AlterColumn<decimal>(
                name: "profile_id",
                table: "post_like",
                type: "numeric(20,0)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid"
            );

            migrationBuilder.AlterColumn<decimal>(
                name: "post_id",
                table: "post_like",
                type: "numeric(20,0)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid"
            );

            migrationBuilder.AlterColumn<decimal>(
                name: "id",
                table: "post_like",
                type: "numeric(20,0)",
                nullable: false,
                comment: "Идентификатор",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "uuidv7()"
            );

            migrationBuilder.AlterColumn<decimal>(
                name: "post_id",
                table: "post_categories",
                type: "numeric(20,0)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid"
            );

            migrationBuilder.AlterColumn<decimal>(
                name: "categories_id",
                table: "post_categories",
                type: "numeric(20,0)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid"
            );

            migrationBuilder.AlterColumn<decimal>(
                name: "author_id",
                table: "post",
                type: "numeric(20,0)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid"
            );

            migrationBuilder.AlterColumn<decimal>(
                name: "id",
                table: "post",
                type: "numeric(20,0)",
                nullable: false,
                comment: "Идентификатор",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "uuidv7()"
            );

            migrationBuilder.AlterColumn<decimal>(
                name: "id",
                table: "category",
                type: "numeric(20,0)",
                nullable: false,
                comment: "Идентификатор",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "uuidv7()"
            );

            migrationBuilder.AlterColumn<decimal>(
                name: "id",
                table: "blog_subscription",
                type: "numeric(20,0)",
                nullable: false,
                comment: "Идентификатор",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "uuidv7()"
            );
        }
    }
}
