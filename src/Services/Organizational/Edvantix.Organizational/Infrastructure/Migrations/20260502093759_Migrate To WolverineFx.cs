using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Edvantix.Organizational.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MigrateToWolverineFx : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_group_members_group_roles_group_role_id",
                table: "group_members"
            );

            migrationBuilder.DropTable(name: "group_role_permission");

            migrationBuilder.DropTable(name: "outbox_message");

            migrationBuilder.DropTable(name: "group_roles");

            migrationBuilder.DropTable(name: "inbox_state");

            migrationBuilder.DropTable(name: "outbox_state");

            migrationBuilder.DropIndex(name: "ix_permissions_feature_name", table: "permissions");

            migrationBuilder.DropIndex(
                name: "ix_group_members_group_role_id",
                table: "group_members"
            );

            migrationBuilder.DropColumn(name: "group_role_id", table: "group_members");

            migrationBuilder.RenameColumn(
                name: "feature",
                table: "permissions",
                newName: "feature_name"
            );

            migrationBuilder.AddColumn<string>(
                name: "code",
                table: "permissions",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: ""
            );

            migrationBuilder.AddColumn<string>(
                name: "feature_code",
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_permissions_service_code_feature_code_code",
                table: "permissions"
            );

            migrationBuilder.DropColumn(name: "code", table: "permissions");

            migrationBuilder.DropColumn(name: "feature_code", table: "permissions");

            migrationBuilder.DropColumn(name: "service_code", table: "permissions");

            migrationBuilder.RenameColumn(
                name: "feature_name",
                table: "permissions",
                newName: "feature"
            );

            migrationBuilder.AddColumn<Guid>(
                name: "group_role_id",
                table: "group_members",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000")
            );

            migrationBuilder.CreateTable(
                name: "group_roles",
                columns: table => new
                {
                    id = table.Column<Guid>(
                        type: "uuid",
                        nullable: false,
                        defaultValueSql: "uuidv7()"
                    ),
                    code = table.Column<string>(
                        type: "character varying(50)",
                        maxLength: 50,
                        nullable: false
                    ),
                    description = table.Column<string>(
                        type: "character varying(100)",
                        maxLength: 100,
                        nullable: true
                    ),
                    is_deleted = table.Column<bool>(
                        type: "boolean",
                        nullable: false,
                        comment: "Признак удаленной записи"
                    ),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_group_roles", x => x.id);
                }
            );

            migrationBuilder.CreateTable(
                name: "inbox_state",
                columns: table => new
                {
                    id = table
                        .Column<long>(type: "bigint", nullable: false)
                        .Annotation(
                            "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                        ),
                    consumed = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    consumer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    delivered = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    expiration_time = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    last_sequence_number = table.Column<long>(type: "bigint", nullable: true),
                    lock_id = table.Column<Guid>(type: "uuid", nullable: false),
                    message_id = table.Column<Guid>(type: "uuid", nullable: false),
                    receive_count = table.Column<int>(type: "integer", nullable: false),
                    received = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    row_version = table.Column<byte[]>(
                        type: "bytea",
                        rowVersion: true,
                        nullable: true
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_inbox_state", x => x.id);
                    table.UniqueConstraint(
                        "ak_inbox_state_message_id_consumer_id",
                        x => new { x.message_id, x.consumer_id }
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "outbox_state",
                columns: table => new
                {
                    outbox_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    delivered = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    last_sequence_number = table.Column<long>(type: "bigint", nullable: true),
                    lock_id = table.Column<Guid>(type: "uuid", nullable: false),
                    row_version = table.Column<byte[]>(
                        type: "bytea",
                        rowVersion: true,
                        nullable: true
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_outbox_state", x => x.outbox_id);
                }
            );

            migrationBuilder.CreateTable(
                name: "group_role_permission",
                columns: table => new
                {
                    group_role_id = table.Column<Guid>(type: "uuid", nullable: false),
                    permission_id = table.Column<Guid>(type: "uuid", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "pk_group_role_permission",
                        x => new { x.group_role_id, x.permission_id }
                    );
                    table.ForeignKey(
                        name: "fk_group_role_permission_group_roles_group_role_id",
                        column: x => x.group_role_id,
                        principalTable: "group_roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "fk_group_role_permission_permissions_permission_id",
                        column: x => x.permission_id,
                        principalTable: "permissions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "outbox_message",
                columns: table => new
                {
                    sequence_number = table
                        .Column<long>(type: "bigint", nullable: false)
                        .Annotation(
                            "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                        ),
                    body = table.Column<string>(type: "text", nullable: false),
                    content_type = table.Column<string>(
                        type: "character varying(256)",
                        maxLength: 256,
                        nullable: false
                    ),
                    conversation_id = table.Column<Guid>(type: "uuid", nullable: true),
                    correlation_id = table.Column<Guid>(type: "uuid", nullable: true),
                    destination_address = table.Column<string>(
                        type: "character varying(256)",
                        maxLength: 256,
                        nullable: true
                    ),
                    enqueue_time = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    expiration_time = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    fault_address = table.Column<string>(
                        type: "character varying(256)",
                        maxLength: 256,
                        nullable: true
                    ),
                    headers = table.Column<string>(type: "text", nullable: true),
                    inbox_consumer_id = table.Column<Guid>(type: "uuid", nullable: true),
                    inbox_message_id = table.Column<Guid>(type: "uuid", nullable: true),
                    initiator_id = table.Column<Guid>(type: "uuid", nullable: true),
                    message_id = table.Column<Guid>(type: "uuid", nullable: false),
                    message_type = table.Column<string>(type: "text", nullable: false),
                    outbox_id = table.Column<Guid>(type: "uuid", nullable: true),
                    properties = table.Column<string>(type: "text", nullable: true),
                    request_id = table.Column<Guid>(type: "uuid", nullable: true),
                    response_address = table.Column<string>(
                        type: "character varying(256)",
                        maxLength: 256,
                        nullable: true
                    ),
                    sent_time = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    source_address = table.Column<string>(
                        type: "character varying(256)",
                        maxLength: 256,
                        nullable: true
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_outbox_message", x => x.sequence_number);
                    table.ForeignKey(
                        name: "fk_outbox_message_inbox_state_inbox_message_id_inbox_consumer_",
                        columns: x => new { x.inbox_message_id, x.inbox_consumer_id },
                        principalTable: "inbox_state",
                        principalColumns: new[] { "message_id", "consumer_id" }
                    );
                    table.ForeignKey(
                        name: "fk_outbox_message_outbox_state_outbox_id",
                        column: x => x.outbox_id,
                        principalTable: "outbox_state",
                        principalColumn: "outbox_id"
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "ix_permissions_feature_name",
                table: "permissions",
                columns: new[] { "feature", "name" },
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "ix_group_members_group_role_id",
                table: "group_members",
                column: "group_role_id"
            );

            migrationBuilder.CreateIndex(
                name: "ix_group_role_permission_permission_id",
                table: "group_role_permission",
                column: "permission_id"
            );

            migrationBuilder.CreateIndex(
                name: "ix_group_roles_organization_id_code",
                table: "group_roles",
                columns: new[] { "organization_id", "code" },
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "ix_inbox_state_delivered",
                table: "inbox_state",
                column: "delivered"
            );

            migrationBuilder.CreateIndex(
                name: "ix_outbox_message_enqueue_time",
                table: "outbox_message",
                column: "enqueue_time"
            );

            migrationBuilder.CreateIndex(
                name: "ix_outbox_message_expiration_time",
                table: "outbox_message",
                column: "expiration_time"
            );

            migrationBuilder.CreateIndex(
                name: "ix_outbox_message_inbox_message_id_inbox_consumer_id_sequence_",
                table: "outbox_message",
                columns: new[] { "inbox_message_id", "inbox_consumer_id", "sequence_number" },
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "ix_outbox_message_outbox_id_sequence_number",
                table: "outbox_message",
                columns: new[] { "outbox_id", "sequence_number" },
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "ix_outbox_state_created",
                table: "outbox_state",
                column: "created"
            );

            migrationBuilder.AddForeignKey(
                name: "fk_group_members_group_roles_group_role_id",
                table: "group_members",
                column: "group_role_id",
                principalTable: "group_roles",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict
            );
        }
    }
}
