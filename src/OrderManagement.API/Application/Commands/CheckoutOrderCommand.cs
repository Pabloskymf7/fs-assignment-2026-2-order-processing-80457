using MediatR;
using Shared.DTOs;

namespace OrderManagement.API.Application.Commands;

public record CheckoutOrderCommand(
    Guid CustomerId,
    List<CartItemDto> Items
) : IRequest<Guid>;