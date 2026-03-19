# Phase 2: Organizations — Permission Cache - Context

**Gathered:** 2026-03-19
**Status:** Ready for planning

<domain>
## Phase Boundary

Organizations service предоставляет gRPC CheckPermission endpoint, кешируемый в HybridCache (L1+L2 Redis), с tag-based инвалидацией при изменении ролей/assignments. При любом изменении публикуется UserPermissionsInvalidated через MassTransit outbox. Downstream сервисы (Scheduling, Payments) подключают IAuthorizationHandler из Chassis, который вызывает gRPC CheckPermission, и объявляют RequireAuthorization политики на endpoints. Scheduling и Payments service не создаются в этой фазе — только Organizations + Chassis authorization extension.

</domain>

<decisions>
## Implementation Decisions

### Cache architecture
- Кеш живёт только в Organizations (downstream сервисы всегда идут через gRPC)
- L1 = in-memory (30 секунд TTL), L2 = Redis (5 минут TTL) — укладывается в success criteria ≤60с
- Cache key: `perm:{userId}:{schoolId}:{permission}` (отдельный ключ на каждую тройку)
- Cache tag: `user:{userId}:{schoolId}` — RemoveByTagAsync сбрасывает всё для пользователя в школе

### gRPC CheckPermission endpoint
- Новый gRPC-сервер в Organizations: `CheckPermission(userId, schoolId, permission) → bool`
- Proto файл в `src/Services/Organizations/Edvantix.Organizations/Grpc/Protos/organizations/v1/permissions.proto`
- Downstream сервисы копируют proto и оборачивают в интерфейс (паттерн из Phase 1: `IPersonaProfileService`)

### GET /permissions endpoint (cache priming)
- REST endpoint: `GET /permissions?userId={id}&schoolId={id}` → `string[]` всех permissions пользователя в школе
- AllowAnonymous — service-to-service только (аналогично RegisterPermissions из Phase 1, TODO mTLS)
- Downstream сервис может вызвать при старте для прогрева L2 Redis

### Invalidation events
- Одно обобщённое событие: `UserPermissionsInvalidated` с payload `{ UserId: Guid, SchoolId: Guid, Timestamp: DateTimeOffset }`
- Публикуется Organizations при: AssignRole, RevokeRole, AssignPermissions (к роли)
- EventMapper (уже есть stub `Infrastructure/EventServices/EventMapper.cs`) расширяется для маппинга domain events → UserPermissionsInvalidated
- Organizations инвалидирует свой кеш немедленно в command handler через RemoveByTagAsync до publish — атомарно с commit

### Authorization в downstream сервисах
- `AddPermissionAuthorization(organizationsGrpcBaseUrl)` extension в Chassis
- ASP.NET Core `IAuthorizationHandler` + `IAuthorizationRequirement` (паттерн как ProfileRegisteredRequirement в Phase 1)
- Endpoint регистрация: `.RequireAuthorization("scheduling.create-slot")` — permission string как имя политики
- При отказе: 403 Forbidden через стандартный ASP.NET Core authz (Problem Details автоматически)

### Claude's Discretion
- Конкретный дизайн proto (streaming vs unary — unary очевидно)
- Имена классов для Requirement/Handler в Chassis
- Kafka topic name для UserPermissionsInvalidated (следовать KebabCase formatter)

</decisions>

<canonical_refs>
## Canonical References

**Downstream agents MUST read these before planning or implementing.**

### Requirements
- `.planning/REQUIREMENTS.md` §ORG-07–ORG-09 — полный набор требований к CheckPermission, кешу, инвалидации
- `.planning/ROADMAP.md` §Phase 2 — Success Criteria (3 критерия: корректность, cache hit, 60с инвалидация)

### Existing patterns (follow these)
- `src/BuildingBlocks/Edvantix.Chassis/Caching/IHybridCache.cs` — существующая cache абстракция (GetOrCreateAsync, RemoveByTagAsync)
- `src/BuildingBlocks/Edvantix.Chassis/Caching/HybridCacheService.cs` — существующая реализация поверх Microsoft.Extensions.Caching.Hybrid
- `src/BuildingBlocks/Edvantix.Chassis/EventBus/Extensions.cs` — MassTransit event bus + Kafka, паттерн IConsumer<T>
- `src/BuildingBlocks/Edvantix.Chassis/Security/Authorization/` — ProfileRegisteredRequirement паттерн для IAuthorizationHandler
- `src/Services/Organizations/Edvantix.Organizations/Infrastructure/EventServices/EventMapper.cs` — stub, расширить для UserPermissionsInvalidated
- `src/Services/Organizations/Edvantix.Organizations/Grpc/` — существующий gRPC client паттерн (для нового gRPC сервера: инвертировать)
- `src/Services/Organizations/Edvantix.Organizations/Grpc/Protos/persona/v1/profile.proto` — образец структуры proto файла
- `.planning/phases/01-organizations-rbac-core/01-CONTEXT.md` — Phase 1 решения (domain model, паттерны, RegisterPermissions endpoint)

</canonical_refs>

<code_context>
## Existing Code Insights

### Reusable Assets
- `IHybridCache` (Chassis): GetOrCreateAsync + RemoveByTagAsync — готово к использованию в CheckPermission handler
- `MassTransit.EntityFrameworkCore` (уже в Organizations.csproj): AddInboxStateEntity + AddOutboxMessageEntity — outbox готов к конфигурации
- `EventMapper` stub (`Infrastructure/EventServices/EventMapper.cs`): расширить switch для маппинга domain events
- `ProfileRegisteredRequirement` pattern (Chassis Security): шаблон для нового PermissionRequirement + Handler
- Kafka EventBus (Chassis `Extensions.cs`): автообнаружение IConsumer<T> через reflection — достаточно реализовать consumer

### Established Patterns
- **gRPC client**: Organizations копирует proto из Persona, добавляет как `GrpcServices="Client"` в .csproj — для gRPC сервера: `GrpcServices="Server"`
- **AllowAnonymous service endpoints**: RegisterPermissionsEndpoint (Phase 1) — та же конвенция для GET /permissions
- **Feature folders**: новая CheckPermission фича в `Features/Permissions/CheckPermission/`
- **IHybridCache tag pattern**: GetOrCreateAsync с `tags: ["user:{userId}:{schoolId}"]`, RemoveByTagAsync по этому тегу

### Integration Points
- **Redis**: уже в AppHost (AddAzureManagedRedis), Organizations нужно добавить `.WithReference(redis)` в AppHost
- **Organizations DB → EventMapper → MassTransit outbox**: EventDispatchInterceptor уже в Chassis EF, нужно включить outbox в DbContext конфигурации
- **Downstream gRPC clients**: Scheduling и Payments получат AddPermissionAuthorization(grpcUrl) в Extensions.cs — Chassis extension регистрирует gRPC client к Organizations

</code_context>

<specifics>
## Specific Ideas

- Downstream сервисы используют permission string напрямую как имя ASP.NET Core политики: `.RequireAuthorization("organizations.create-role")` — читаемо и не требует отдельной регистрации каждой политики
- GET /permissions endpoint следует точно тому же AllowAnonymous + TODO mTLS паттерну, что RegisterPermissionsEndpoint в Phase 1 — одна конвенция для всех service-to-service endpoints

</specifics>

<deferred>
## Deferred Ideas

- Нет — обсуждение осталось в рамках Phase 2 scope

</deferred>

---

*Phase: 02-organizations-permission-cache*
*Context gathered: 2026-03-19*
