using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edvantix.Organizational.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MigrateGroupLevelToFK : Migration
    {
        // Соответствие строковых значений enum GroupLevel → код уровня в таблице levels.
        // Junior/Teen/Preschool — старые значения; JR/TN/PR — коды в сидере OrganizationCreatedSeedLevelsHandler.
        private const string BackfillSql = """
            -- 1. Гарантируем, что у каждой организации с группами есть все 8 базовых уровней.
            --    Для организаций, созданных до добавления сидера (#367), уровни могут отсутствовать.
            INSERT INTO levels (id, organization_id, code, name, tone, sort_order, is_active, is_deleted)
            SELECT
                uuidv7(),
                orgs.organization_id,
                seed.code,
                seed.name,
                seed.tone,
                seed.sort_order,
                true,
                false
            FROM (
                SELECT DISTINCT organization_id
                FROM groups
                WHERE is_deleted = false
            ) orgs
            CROSS JOIN (VALUES
                ('A1', 'A1 — Начальный',                   'Teal',   CAST(10  AS smallint)),
                ('A2', 'A2 — Базовый',                     'Teal',   CAST(20  AS smallint)),
                ('B1', 'B1 — Средний',                     'Blue',   CAST(30  AS smallint)),
                ('B2', 'B2 — Продвинутый',                 'Blue',   CAST(40  AS smallint)),
                ('C1', 'C1 — Высокий',                     'Indigo', CAST(50  AS smallint)),
                ('JR', 'Дети 7–10 лет',                    'Amber',  CAST(60  AS smallint)),
                ('TN', 'Подростки 11–14 лет',              'Amber',  CAST(70  AS smallint)),
                ('PR', 'Подготовка к экзаменам',           'Violet', CAST(80  AS smallint))
            ) AS seed(code, name, tone, sort_order)
            WHERE NOT EXISTS (
                SELECT 1
                FROM levels l
                WHERE l.organization_id = orgs.organization_id
                  AND l.code = seed.code
                  AND l.is_deleted = false
            );

            -- 2. Заполняем level_id по соответствию enum-строки → коду уровня.
            UPDATE groups g
            SET level_id = l.id
            FROM levels l
            WHERE l.organization_id = g.organization_id
              AND l.is_deleted = false
              AND l.code = CASE g.level
                  WHEN 'A1'        THEN 'A1'
                  WHEN 'A2'        THEN 'A2'
                  WHEN 'B1'        THEN 'B1'
                  WHEN 'B2'        THEN 'B2'
                  WHEN 'C1'        THEN 'C1'
                  WHEN 'Junior'    THEN 'JR'
                  WHEN 'Teen'      THEN 'TN'
                  WHEN 'Preschool' THEN 'PR'
                  ELSE NULL
              END;

            -- 3. Защитная проверка: не должно остаться не-удалённых групп без level_id.
            DO $$
            BEGIN
                IF EXISTS (
                    SELECT 1 FROM groups WHERE level_id IS NULL AND is_deleted = false
                ) THEN
                    RAISE EXCEPTION
                        'MigrateGroupLevelToFK: найдены не-удалённые группы без level_id после '
                        'бэкфила. Проверьте значения колонки level и наличие уровней для всех '
                        'организаций.';
                END IF;
            END $$;
            """;

        private const string RevertSql = """
            UPDATE groups g
            SET level = COALESCE(
                CASE l.code
                    WHEN 'A1' THEN 'A1'
                    WHEN 'A2' THEN 'A2'
                    WHEN 'B1' THEN 'B1'
                    WHEN 'B2' THEN 'B2'
                    WHEN 'C1' THEN 'C1'
                    WHEN 'JR' THEN 'Junior'
                    WHEN 'TN' THEN 'Teen'
                    WHEN 'PR' THEN 'Preschool'
                END,
                'B1'
            )
            FROM levels l
            WHERE l.id = g.level_id;
            """;

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Шаг 1: Добавляем nullable-колонку level_id.
            migrationBuilder.AddColumn<Guid>(
                name: "level_id",
                table: "groups",
                type: "uuid",
                nullable: true
            );

            // Шаг 2–3: Сидируем уровни (если отсутствуют), бэкфилим level_id, проверяем результат.
            migrationBuilder.Sql(BackfillSql);

            // Шаг 4: Устанавливаем NOT NULL.
            migrationBuilder.AlterColumn<Guid>(
                name: "level_id",
                table: "groups",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true
            );

            // Шаг 5: Удаляем старый строковый столбец level.
            migrationBuilder.DropIndex(name: "ix_groups_organization_id_status", table: "groups");

            migrationBuilder.DropColumn(name: "level", table: "groups");

            // Шаг 6: FK на таблицу levels (Restrict — нельзя удалить уровень, если есть группы).
            migrationBuilder.AddForeignKey(
                name: "fk_groups_levels_level_id",
                table: "groups",
                column: "level_id",
                principalTable: "levels",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict
            );

            // Шаг 7: Составной индекс (organization_id, level_id, status) для фильтрации списка групп.
            migrationBuilder.CreateIndex(
                name: "ix_groups_organization_id_level_id_status",
                table: "groups",
                columns: new[] { "organization_id", "level_id", "status" }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "fk_groups_levels_level_id", table: "groups");

            migrationBuilder.DropIndex(
                name: "ix_groups_organization_id_level_id_status",
                table: "groups"
            );

            // Возвращаем строковую колонку level.
            migrationBuilder.AddColumn<string>(
                name: "level",
                table: "groups",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: ""
            );

            // Восстанавливаем значения level из level_id через коды levels.
            migrationBuilder.Sql(RevertSql);

            migrationBuilder.DropColumn(name: "level_id", table: "groups");

            migrationBuilder.CreateIndex(
                name: "ix_groups_organization_id_status",
                table: "groups",
                columns: new[] { "organization_id", "status" }
            );
        }
    }
}
