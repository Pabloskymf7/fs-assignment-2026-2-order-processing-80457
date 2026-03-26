using MediatR;
using Shared.DTOs;

namespace OrderManagement.API.Application.Queries;

public record GetOrderByIdQuery(Guid OrderId) : IRequest<OrderDto?>;