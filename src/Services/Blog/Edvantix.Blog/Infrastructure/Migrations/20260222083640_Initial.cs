using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edvantix.Blog.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "blog_subscription",
                columns: table => new
                {
                    id = table.Column<decimal>(
                        type: "numeric(20,0)",
                        nullable: false,
                        comment: "Идентификатор"
                    ),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    content_types = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    updated_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_blog_subscription", x => x.id);
                }
            );

            migrationBuilder.CreateTable(
                name: "category",
                columns: table => new
                {
                    id = table.Column<decimal>(
                        type: "numeric(20,0)",
                        nullable: false,
                        comment: "Идентификатор"
                    ),
                    name = table.Column<string>(
                        type: "character varying(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    slug = table.Column<string>(
                        type: "character varying(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    description = table.Column<string>(
                        type: "character varying(1000)",
                        maxLength: 1000,
                        nullable: true
                    ),
                    created_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_category", x => x.id);
                }
            );

            migrationBuilder.CreateTable(
                name: "post",
                columns: table => new
                {
                    id = table.Column<decimal>(
                        type: "numeric(20,0)",
                        nullable: false,
                        comment: "Идентификатор"
                    ),
                    title = table.Column<string>(
                        type: "character varying(500)",
                        maxLength: 500,
                        nullable: false
                    ),
                    slug = table.Column<string>(
                        type: "character varying(255)",
                        maxLength: 255,
                        nullable: false
                    ),
                    content = table.Column<string>(type: "text", nullable: false),
                    summary = table.Column<string>(
                        type: "character varying(1000)",
                        maxLength: 1000,
                        nullable: true
                    ),
                    type = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    is_premium = table.Column<bool>(
                        type: "boolean",
                        nullable: false,
                        defaultValue: false
                    ),
                    author_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    published_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    scheduled_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    cover_image_url = table.Column<string>(
                        type: "character varying(1000)",
                        maxLength: 1000,
                        nullable: true
                    ),
                    likes_count = table.Column<int>(
                        type: "integer",
                        nullable: false,
                        defaultValue: 0
                    ),
                    created_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    updated_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_post", x => x.id);
                }
            );

            migrationBuilder.CreateTable(
                name: "tag",
                columns: table => new
                {
                    id = table.Column<decimal>(
                        type: "numeric(20,0)",
                        nullable: false,
                        comment: "Идентификатор"
                    ),
                    name = table.Column<string>(
                        type: "character varying(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    slug = table.Column<string>(
                        type: "character varying(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    created_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tag", x => x.id);
                }
            );

            migrationBuilder.CreateTable(
                name: "post_categories",
                columns: table => new
                {
                    categories_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    post_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_post_categories", x => new { x.categories_id, x.post_id });
                    table.ForeignKey(
                        name: "fk_post_categories_category_categories_id",
                        column: x => x.categories_id,
                        principalTable: "category",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "fk_post_categories_post_post_id",
                        column: x => x.post_id,
                        principalTable: "post",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "post_like",
                columns: table => new
                {
                    id = table.Column<decimal>(
                        type: "numeric(20,0)",
                        nullable: false,
                        comment: "Идентификатор"
                    ),
                    post_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    profile_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    created_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_post_like", x => x.id);
                    table.ForeignKey(
                        name: "fk_post_like_post_post_id",
                        column: x => x.post_id,
                        principalTable: "post",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "post_tags",
                columns: table => new
                {
                    post_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    tags_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_post_tags", x => new { x.post_id, x.tags_id });
                    table.ForeignKey(
                        name: "fk_post_tags_post_post_id",
                        column: x => x.post_id,
                        principalTable: "post",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "fk_post_tags_tag_tags_id",
                        column: x => x.tags_id,
                        principalTable: "tag",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "ix_blog_subscription_user_id",
                table: "blog_subscription",
                column: "user_id",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "ix_category_name",
                table: "category",
                column: "name"
            );

            migrationBuilder.CreateIndex(
                name: "ix_category_slug",
                table: "category",
                column: "slug",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "ix_post_author_id",
                table: "post",
                column: "author_id"
            );

            migrationBuilder.CreateIndex(
                name: "ix_post_published_at",
                table: "post",
                column: "published_at"
            );

            migrationBuilder.CreateIndex(
                name: "ix_post_slug",
                table: "post",
                column: "slug",
                unique: true
            );

            migrationBuilder.CreateIndex(name: "ix_post_status", table: "post", column: "status");

            migrationBuilder.CreateIndex(name: "ix_post_type", table: "post", column: "type");

            migrationBuilder.CreateIndex(
                name: "ix_post_categories_post_id",
                table: "post_categories",
                column: "post_id"
            );

            migrationBuilder.CreateIndex(
                name: "ix_post_like_post_id_profile_id",
                table: "post_like",
                columns: new[] { "post_id", "profile_id" },
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "ix_post_tags_tags_id",
                table: "post_tags",
                column: "tags_id"
            );

            migrationBuilder.CreateIndex(name: "ix_tag_name", table: "tag", column: "name");

            migrationBuilder.CreateIndex(
                name: "ix_tag_slug",
                table: "tag",
                column: "slug",
                unique: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "blog_subscription");

            migrationBuilder.DropTable(name: "post_categories");

            migrationBuilder.DropTable(name: "post_like");

            migrationBuilder.DropTable(name: "post_tags");

            migrationBuilder.DropTable(name: "category");

            migrationBuilder.DropTable(name: "post");

            migrationBuilder.DropTable(name: "tag");
        }
    }
}
