using MediatR;

namespace OrderManagement.API.Application.Commands;

public record CancelOrderCommand(Guid OrderId) : IRequest<bool>;
