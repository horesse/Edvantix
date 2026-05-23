using System.Net;
using Grpc.Core;

namespace Edvantix.Organizational.UnitTests.Grpc.Context;

public sealed class TestServerCallContext(
    Metadata? requestHeaders = null,
    DateTime? deadline = null,
    CancellationToken cancellationToken = default
) : ServerCallContext
{
    protected override string MethodCore => "test";

    protected override string HostCore => "localhost";

    protected override string PeerCore => IPAddress.Loopback.ToString();

    protected override DateTime DeadlineCore { get; } = deadline ?? DateTime.UtcNow.AddMinutes(5);

    protected override Metadata RequestHeadersCore { get; } = requestHeaders ?? [];

    protected override CancellationToken CancellationTokenCore => cancellationToken;

    protected override Metadata ResponseTrailersCore { get; } = [];

    protected override Status StatusCore { get; set; }

    protected override WriteOptions? WriteOptionsCore
    {
        get => null;
        set => _ = value;
    }

    protected override AuthContext AuthContextCore { get; } = new("anonymous", []);

    protected override ContextPropagationToken CreatePropagationTokenCore(
        ContextPropagationOptions? options
    )
    {
        throw new NotImplementedException();
    }

    protected override Task WriteResponseHeadersAsyncCore(Metadata responseHeaders) =>
        Task.CompletedTask;
}
