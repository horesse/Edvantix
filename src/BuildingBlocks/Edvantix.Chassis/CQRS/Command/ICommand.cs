using MediatR;

namespace Edvantix.Chassis.CQRS.Command;

public interface ICommand<out TResponse> : IRequest<TResponse>;
