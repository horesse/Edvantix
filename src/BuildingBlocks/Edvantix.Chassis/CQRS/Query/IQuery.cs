using MediatR;

namespace Edvantix.Chassis.CQRS.Query;

public interface IQuery<out TResponse> : IRequest<TResponse>;
