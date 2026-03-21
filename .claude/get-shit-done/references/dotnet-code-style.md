# .NET Code Style Rules — Edvantix

Project-specific conventions enforced during code review. GSD agents MUST follow these when generating .NET code.

## Proto Files

- **Source of truth:** `.proto` files live in the service that **implements** the gRPC server.
  - Server: `<Protobuf Include="Grpc\Protos\{service}\v1\{name}.proto" GrpcServices="Server" />`
  - Client (other service): reference via `Include=` with relative path **and** `Link=` — no copies.
    ```xml
    <Protobuf
      Include="..\..\{OwnerService}\{OwnerService}.csproj\Grpc\Protos\{service}\v1\{name}.proto"
      Link="Grpc\Protos\{service}\v1\{name}.proto"
      GrpcServices="Client"
    />
    ```
- **Never** put `.proto` files in `Edvantix.Chassis` or any shared BuildingBlock — protos belong to the owning service.
- **Never** copy `.proto` files — always use `Link=`.

## EF Core Entity Configurations

- **Never** use `builder.ToTable("table_name")` — `UseSnakeCaseNamingConvention()` is already configured in all DbContextFactory files and handles naming automatically.
- **Never** use `builder.Property(x => x.Prop).HasColumnName("col_name")` for the same reason.
- Exception: raw SQL strings in `HasFilter(...)` must use the actual DB column name (snake_case) since they bypass EF naming conventions.

## Repositories

- Repositories expose **generic query methods** via `Specification<T>`, not ad-hoc named query methods.
  - ✓ `Task<List<T>> ListAsync(Specification<T> spec, CancellationToken ct)`
  - ✓ `Task<T?> FindAsync(Specification<T> spec, CancellationToken ct)`
  - ✗ `GetAllAsync()`, `FindByIdWithMembersAsync()`, `GetByNamesAsync()`, `GetBySchoolAsync()`
- Simple by-ID convenience methods (`FindByIdAsync(Guid id)`) are acceptable.
- Specifications live in `Domain/AggregatesModel/{Aggregate}/` next to the aggregate.
- Use `SpecificationEvaluator.Instance.GetQuery(dbSet, spec)` in repository implementations — see `Blog/Infrastructure/Repositories/PostRepository.cs` as reference.

## Mapping

- **Never** build DTOs inline in query/command handlers (`new MyDto(entity.Id, entity.Name, ...)`).
- Use `Edvantix.Chassis.Mapper.Mapper<TSource, TDestination>` — one mapper class per mapping.
- Register via `services.AddMapper(typeof(IServiceApiMarker))` in `Extensions.cs` — scans the assembly automatically.
- When mapping requires extra context (e.g., role-based field shaping), create a mapping context record as the source type:
  ```csharp
  public sealed record MyMappingContext(Entity Entity, bool IsAdmin);
  public sealed class MyMapper : Mapper<MyMappingContext, MyDto> { ... }
  ```
- Mappers live in `Features/{FeatureName}/` next to the handler that uses them.

## Domain & Integration Events

- Domain events: `Domain/Events/{EventName}.cs`
- Domain event handlers: `Domain/EventHandlers/{HandlerName}.cs`
- Integration events: `IntegrationEvents/{EventName}.cs`
- Integration event mapping: `Infrastructure/EventServices/EventMapper.cs`

## gRPC Services

- **Never** use HTTP clients (`IHttpClientFactory`, `HttpClient`) to call gRPC endpoints — always use the generated gRPC client (`{Service}GrpcServiceClient`).
- gRPC clients are injected via `AddGrpcClient<T>()` in `Grpc/Extensions.cs`.
