using Edvantix.Organizational.Domain.AggregatesModel.LevelAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.OrganizationMemberAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.OrganizationRoleAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.PermissionAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.StudentStatusAggregate;
using Edvantix.Organizational.Domain.Events;

namespace Edvantix.Organizational.Domain.EventHandlers;

/// <summary>
/// Создаёт базовую матрицу ролей, назначает владельца организации,
/// сидирует системные статусы студентов и базовые уровни обучения.
/// Выполняется в той же транзакционной области после сохранения агрегата Organization.
/// </summary>
internal sealed class OrganizationCreatedDomainEventHandler(
    IOrganizationRoleRepository roleRepository,
    IOrganizationMemberRepository memberRepository,
    IPermissionRepository permissionRepository,
    IStudentStatusRepository studentStatusRepository,
    ILevelRepository levelRepository
) : INotificationHandler<OrganizationCreatedDomainEvent>
{
    /// <summary>
    /// Набор уровней, создаваемых по умолчанию для каждой новой организации.
    /// Включает стандартные уровни CEFR (A1–C1) и возрастные/специальные группы.
    /// </summary>
    private static readonly (string Code, string Name, LevelTone Tone, short Order)[] DefaultLevels =
    [
        ("A1", "A1 — Начальный", LevelTone.Teal, 10),
        ("A2", "A2 — Базовый", LevelTone.Teal, 20),
        ("B1", "B1 — Средний", LevelTone.Blue, 30),
        ("B2", "B2 — Продвинутый", LevelTone.Blue, 40),
        ("C1", "C1 — Высокий", LevelTone.Indigo, 50),
        ("JR", "Дети 7–10 лет", LevelTone.Amber, 60),
        ("TN", "Подростки 11–14 лет", LevelTone.Amber, 70),
        ("PR", "Подготовка к экзаменам", LevelTone.Violet, 80),
    ];

    public async ValueTask Handle(
        OrganizationCreatedDomainEvent notification,
        CancellationToken cancellationToken
    )
    {
        var allPermissions = await permissionRepository.GetAllAsync(cancellationToken);

        var orgRoles = OrganizationDefaultRolesFactory.CreateFor(
            notification.OrganizationId,
            allPermissions
        );

        await roleRepository.AddRangeAsync(orgRoles, cancellationToken);

        var ownerRole = orgRoles.First(r => r.IsSystem);
        var ownerMember = new OrganizationMember(
            notification.OrganizationId,
            notification.OwnerProfileId,
            ownerRole.Id,
            DateOnly.FromDateTime(DateTime.UtcNow)
        );

        await memberRepository.AddAsync(ownerMember, cancellationToken);

        // Создаём 4 системных статуса студентов для новой организации
        var defaultStatuses = DefaultStudentStatusesFactory.CreateFor(notification.OrganizationId);
        await studentStatusRepository.AddRangeAsync(defaultStatuses, cancellationToken);

        // Сидируем 8 базовых уровней обучения (A1–C1, JR, TN, PR).
        // Ранее это делал Groups-сервис через integration event; теперь Organizational владеет уровнями.
        foreach (var (code, name, tone, order) in DefaultLevels)
        {
            var level = new Level(
                notification.OrganizationId,
                LevelCode.From(code),
                name,
                description: null,
                tone,
                order
            );
            await levelRepository.AddAsync(level, cancellationToken);
        }

        await memberRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}
