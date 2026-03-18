# Phase 1: Organizations — RBAC Core - Context

**Gathered:** 2026-03-18
**Status:** Ready for planning

<domain>
## Phase Boundary

Создать Organizations service с нуля: кастомные роли, назначение permissions на роли, назначение ролей пользователям — всё tenant-изолировано по schoolId. Ввести ITenantContext + EF Core global query filter convention и architecture test, который наследуют Scheduling и Payments. gRPC CheckPermission и кеш — Phase 2.

</domain>

<decisions>
## Implementation Decisions

### Tenant isolation convention
- Tenant context передаётся через HTTP-заголовок `X-School-Id` в каждом запросе
- Пользователь может состоять в нескольких школах одновременно — school context per-request, не per-JWT
- ITenantContext извлекается в ASP.NET Core middleware, регистрируется как scoped DI, DbContext получает через конструктор
- Дополнительно: `IMessagePreProcessor<TMessage>` в Mediator pipeline может использоваться для дополнительной tenant-валидации перед handler
- Architecture test (NetArchTest.Rules): любая ITenanted-сущность в любом сервисе должна иметь зарегистрированный EF Core global query filter по schoolId — иначе билд падает

### Permission string format
- Формат: `service.verb-noun` (kebab-case), например: `organizations.create-role`, `scheduling.create-slot`
- Объявляются в static class-константах, например `OrganizationsPermissions.CreateRole = "organizations.create-role"`
- Каталог permission strings хранится в БД Organizations service (таблица Permissions)
- Сервисы регистрируют свои permissions при старте через upsert-вызов к Organizations API
- Перед сохранением AssignPermissionsToRole — валидация: permission string должен существовать в каталоге

### Role management API
- Soft-delete для ролей (ISoftDelete паттерн уже есть в SharedKernel), query filter скрывает удалённые
- DELETE /roles/{id} возвращает 409 Conflict если роль назначена пользователям — владелец должен сначала отозвать роль
- GET /roles → [{id, name}] — без permissions в списке
- GET /roles/{id}/permissions — отдельный endpoint для permissions роли

### User identity in assignments
- UserRoleAssignment хранит profileId из Persona (не Keycloak sub)
- profile_id claim уже есть в JWT (Persona добавляет)
- Пользователь может иметь несколько ролей в одной школе (несколько записей UserRoleAssignment)
- Схема: UserRoleAssignment(profileId, schoolId, roleId) — unique constraint на (profileId, schoolId, roleId)
- При назначении роли Organizations валидирует profileId через gRPC к Persona service

### Claude's Discretion
- Именование таблиц и колонок в EF Core конфигурациях
- Структура папок внутри Organizations service (следовать паттерну Persona)
- Конкретный дизайн IMessagePreProcessor для tenant-валидации
- Формат error responses (следовать существующим паттернам Chassis)

</decisions>

<canonical_refs>
## Canonical References

**Downstream agents MUST read these before planning or implementing.**

### Tenant isolation
- `.planning/REQUIREMENTS.md` §ORG-10 — EF Core global query filter по schoolId во всех агрегатах
- `.planning/ROADMAP.md` §Phase 1 — Success Criteria 5: architecture test fails build

### RBAC model
- `.planning/REQUIREMENTS.md` §ORG-01–ORG-06 — полный набор требований к ролям и assignments
- `.planning/PROJECT.md` §Context — описание RBAC-модели и AuthN vs AuthZ разделения

### Existing patterns (follow these)
- `src/Services/Persona/Edvantix.Persona/Infrastructure/PersonaDbContext.cs` — паттерн DbContext + IUnitOfWork
- `src/Services/Persona/Edvantix.Persona/Infrastructure/EntityConfigurations/` — UseDefaultConfiguration() + HasQueryFilter паттерн
- `src/BuildingBlocks/Edvantix.SharedKernel/SeedWork/` — Entity<TId>, IAggregateRoot, ISoftDelete, ValueObject
- `src/BuildingBlocks/Edvantix.Chassis/EF/` — BaseDbContext, IUnitOfWork, IRepository
- `src/BuildingBlocks/Edvantix.Chassis/Security/Authorization/` — ProfileRegisteredRequirement паттерн для кастомных authorization handlers
- `src/BuildingBlocks/Edvantix.Chassis/CQRS/Pipelines/` — существующие behaviors (Validation, Logging, Activity)

</canonical_refs>

<code_context>
## Existing Code Insights

### Reusable Assets
- `Entity<TId>` + `IAggregateRoot` (SharedKernel): base classes для Role, Permission, UserRoleAssignment aggregates
- `ISoftDelete` (SharedKernel): soft-delete паттерн для ролей
- `IUnitOfWork` + `IRepository<T>` (Chassis): data access паттерн
- `UseDefaultConfiguration()` (Chassis EF): базовая entity конфигурация
- `ValidationBehavior<TRequest, TResponse>` (Chassis): уже зарегистрирован в pipeline
- `ProfileRegisteredRequirement` паттерн: образец для TenantAccessRequirement
- `Mediator.Abstractions` v3.0.1 (не MediatR): IMessagePreProcessor для tenant extraction
- MassTransit outbox configuration: `AddInboxStateEntity()`, `AddOutboxMessageEntity()` — для Phase 2

### Established Patterns
- **Feature folders**: каждая фича в отдельной папке Features/{FeatureName}/Commands/ + Queries/
- **EF конфигурации**: `ApplyConfigurationsFromAssembly(typeof(DbContext).Assembly)` — конфигурации через `IEntityTypeConfiguration<T>`
- **HasQueryFilter**: применяется per-entity в EntityTypeConfiguration (не в DbContext)
- **GlobalUsings.cs**: все сервисы используют global using imports
- **Grpc folder**: Persona уже имеет gRPC интеграцию — паттерн для gRPC клиента к Persona

### Integration Points
- **X-School-Id header**: новый header, все сервисы должны его передавать — Organizations вводит конвенцию
- **Persona gRPC**: Organizations должен вызывать Persona для валидации profileId (см. `src/Services/Persona/Edvantix.Persona/Grpc/`)
- **Permission registration on startup**: сервисы вызывают Organizations API при старте для регистрации своих permissions
- **Aspire app host**: новый Organizations service добавляется в app model

</code_context>

<specifics>
## Specific Ideas

- Keycloak sub claim НЕ используется в UserRoleAssignment — используется profileId из Persona (profile_id claim в JWT)
- Organizations service НЕ хранит список пользователей школы — только assignments (кто какую роль имеет)
- Tenant context per-request через X-School-Id header (не per-JWT) — ключевое архитектурное решение для multi-school пользователей

</specifics>

<deferred>
## Deferred Ideas

- Валидация что пользователь действительно принадлежит школе при назначении роли — не требование Phase 1, Organizations хранит только assignments
- Список пользователей школы (member management) — отдельная фича, не в Phase 1
- Шаблоны ролей (predefined role sets) — ORG-V2-02, defer в v2

</deferred>

---

*Phase: 01-organizations-rbac-core*
*Context gathered: 2026-03-18*
