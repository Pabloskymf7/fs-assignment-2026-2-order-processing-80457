using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderManagement.API.Infrastructure.Data;
using Shared.DTOs;

namespace OrderManagement.API.Application.Queries;

public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderDto?>
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public GetOrderByIdQueryHandler(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<OrderDto?> Handle(GetOrderByIdQuery request, CancellationToken ct)
    {
        var order = await _db.Orders
            .Include(o => o.Items)
            .Include(o => o.Payment)
            .Include(o => o.Shipment)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, ct);

        return order is null ? null : _mapper.Map<OrderDto>(order);
    }
}