using MediatR;
using Shared.DTOs;

namespace OrderManagement.API.Application.Queries;

public record GetOrdersQuery : IRequest<List<OrderDto>>;