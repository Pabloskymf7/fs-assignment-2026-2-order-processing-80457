using MediatR;
using Shared.DTOs;

namespace OrderManagement.API.Application.Queries;

public record GetCustomerOrdersQuery(Guid CustomerId) : IRequest<List<OrderDto>>;