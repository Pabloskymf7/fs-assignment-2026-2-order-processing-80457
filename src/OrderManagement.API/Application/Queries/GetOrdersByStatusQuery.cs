using MediatR;
using Shared.DTOs;
using Shared.Enums;

namespace OrderManagement.API.Application.Queries;

public record GetOrdersByStatusQuery(OrderStatus Status) : IRequest<List<OrderDto>>;
