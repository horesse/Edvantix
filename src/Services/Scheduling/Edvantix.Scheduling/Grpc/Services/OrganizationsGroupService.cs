using System.Net.Http.Json;

namespace Edvantix.Scheduling.Grpc.Services;

/// <summary>
/// HTTP-based implementation of <see cref="IOrganizationsGroupService"/>.
/// Uses the named "organizations" <see cref="IHttpClientFactory"/> client registered in
/// <c>Extensions/Extensions.cs</c> with Aspire service discovery and standard resilience handler.
///
/// NOTE: This is a temporary HTTP fallback for v1.
/// Plan 03-09 will replace this with gRPC methods on the Organizations service.
/// The interface contract remains unchanged — only the implementation will be swapped.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class OrganizationsGroupService(IHttpClientFactory httpClientFactory)
    : IOrganizationsGroupService
{
    /// <inheritdoc/>
    public async Task<bool> GroupExistsAsync(Guid groupId, CancellationToken ct)
    {
        // Named client is registered as "organizations" in Extensions/Extensions.cs
        // with base address pointing to the Organizations service via Aspire service discovery.
        var client = httpClientFactory.CreateClient("organizations");
        var response = await client.GetAsync($"/v1/groups/{groupId}", ct);

        return response.IsSuccessStatusCode;
    }

    /// <inheritdoc/>
    public async Task<List<Guid>> GetGroupsForStudentAsync(
        Guid schoolId,
        Guid profileId,
        CancellationToken ct
    )
    {
        // Calls GET /v1/groups/student/{profileId}?schoolId={schoolId} on the Organizations service.
        // Plan 03-09 will replace this endpoint call with a proper gRPC method.
        // Returns an empty list on failure to avoid blocking the schedule query.
        var client = httpClientFactory.CreateClient("organizations");

        try
        {
            var response = await client.GetAsync(
                $"/v1/groups/student/{profileId}?schoolId={schoolId}",
                ct
            );

            if (!response.IsSuccessStatusCode)
            {
                return [];
            }

            var groupIds = await response.Content.ReadFromJsonAsync<List<Guid>>(ct);

            return groupIds ?? [];
        }
        catch
        {
            return [];
        }
    }

    /// <inheritdoc/>
    public async Task<GroupInfoDto?> GetGroupAsync(Guid groupId, CancellationToken ct)
    {
        // Calls GET /v1/groups/{groupId} on the Organizations service.
        // Plan 03-09 will replace this with a gRPC GetGroupAsync call.
        var client = httpClientFactory.CreateClient("organizations");

        try
        {
            var response = await client.GetAsync($"/v1/groups/{groupId}", ct);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            // Deserialize only the fields needed for ScheduleSlotDto — Name and Color.
            var dto = await response.Content.ReadFromJsonAsync<GroupResponseDto>(ct);

            return dto is null ? null : new GroupInfoDto(dto.Name, dto.Color);
        }
        catch
        {
            return null;
        }
    }

    // Internal response shape matching the Organizations GET /v1/groups/{id} endpoint.
    private sealed record GroupResponseDto(Guid Id, string Name, int MaxCapacity, string Color);
}
