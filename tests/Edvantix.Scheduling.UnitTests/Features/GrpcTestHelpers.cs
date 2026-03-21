using Grpc.Core;

namespace Edvantix.Scheduling.UnitTests.Features;

/// <summary>
/// Helpers for creating gRPC test doubles without a running gRPC server.
/// <c>AsyncUnaryCall</c> is the return type of generated gRPC client async methods;
/// this factory creates a completed in-memory call wrapping a known response.
/// </summary>
internal static class GrpcTestHelpers
{
    /// <summary>
    /// Creates an <see cref="AsyncUnaryCall{TResponse}"/> that immediately returns
    /// <paramref name="response"/> with a successful status code.
    /// </summary>
    /// <typeparam name="TResponse">The gRPC response message type.</typeparam>
    /// <param name="response">The response payload to return.</param>
    internal static AsyncUnaryCall<TResponse> CreateUnaryCall<TResponse>(TResponse response)
        where TResponse : class =>
        new(
            Task.FromResult(response),
            Task.FromResult(new Metadata()),
            () => Status.DefaultSuccess,
            () => [],
            () => { }
        );
}
