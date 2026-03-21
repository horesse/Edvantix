---
name: dotnet_code_style_rules
description: Project-specific .NET code style rules enforced by the user — proto location, EF config, repositories, mappers, events
type: feedback
---

Follow these rules when generating .NET code for this project:

**Proto files:** Never put .proto in Chassis or shared libs. Proto lives in the owning (server) service. Consumers reference via `<Protobuf Include="...path-to-owner..." Link="..." GrpcServices="Client" />` — no copies.

**EF Core configs:** Never use `builder.ToTable()` or `HasColumnName()`. `UseSnakeCaseNamingConvention()` is already set in all DbContextFactory files. Raw SQL in `HasFilter()` must still use actual snake_case column names.

**Repositories:** No ad-hoc named query methods (`GetAllAsync`, `FindByIdWithMembersAsync`, `GetByNamesAsync`). Use `Specification<T>` with `ListAsync(spec)` and `FindAsync(spec)`. Reference: `Edvantix.Chassis.Specification` + `Blog/Infrastructure/Repositories/PostRepository.cs`.

**Mapping:** No inline `new MyDto(...)` in handlers. Use `Mapper<TSource, TDestination>` from `Edvantix.Chassis.Mapper`. Register with `services.AddMapper(typeof(IApiMarker))`. For context-dependent mapping (e.g. role-based), create a mapping context record as TSource.

**Events:** Domain events → `Domain/Events/`. Event handlers → `Domain/EventHandlers/`. Integration events → `IntegrationEvents/`. Event mapper → `Infrastructure/EventServices/EventMapper.cs`.

**gRPC:** Never use HTTP clients to call gRPC endpoints. Use generated gRPC clients only.

**Why:** Enforced during code review of Phase 03. All violations corrected in commit `8f41066`.

**How to apply:** Before generating any repository/EF config/DTO mapping code, check these rules. Reference Blog service as the canonical example.
