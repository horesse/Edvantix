namespace Edvantix.Scheduling.Grpc.Services;

/// <summary>
/// HTTP-based implementation of <see cref="IOrganizationsGroupService"/>.
/// Validates group existence by calling <c>GET /v1/groups/{groupId}</c> on the Organizations service.
/// A 200 response indicates the group exists; 404 means it does not.
///
/// NOTE: This is a temporary HTTP fallback for v1.
/// Plan 03-09 will replace this with a proper gRPC method on the Organizations service.
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
}
