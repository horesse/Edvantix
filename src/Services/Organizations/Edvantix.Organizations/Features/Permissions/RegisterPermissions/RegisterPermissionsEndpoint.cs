namespace Edvantix.Organizations.Features.Permissions.RegisterPermissions;

/// <summary>
/// POST /v1/permissions/register — upserts a list of permission strings into the global catalogue.
/// Intended for other services to call during their startup to self-register their permission strings.
/// This endpoint is allowed anonymous access because it is called by service identities, not end users.
/// TODO: In production, secure this endpoint with a shared secret or service-to-service mTLS policy.
/// </summary>
public sealed class RegisterPermissionsEndpoint : IEndpoint<NoContent, RegisterPermissionsCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/permissions/register",
                async (
                    RegisterPermissionsCommand command,
                    ISender sender,
                    CancellationToken cancellationToken
                ) => await HandleAsync(command, sender, cancellationToken)
            )
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status422UnprocessableEntity)
            .WithName("RegisterPermissions")
            .WithTags("Permissions")
            .WithSummary("Register permission strings")
            .WithDescription(
                "Upserts permission strings into the global catalogue. Idempotent — existing names are skipped. "
                + "Called by other services during startup to register their own permission catalogues."
            )
            .MapToApiVersion(ApiVersions.V1)
            .AllowAnonymous();
    }

    public async Task<NoContent> HandleAsync(
        RegisterPermissionsCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(command, cancellationToken);
        return TypedResults.NoContent();
    }
}
