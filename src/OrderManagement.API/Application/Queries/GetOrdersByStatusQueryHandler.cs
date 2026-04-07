using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderManagement.API.Infrastructure.Data;
using Shared.DTOs;

namespace OrderManagement.API.Application.Queries;

public class GetOrdersByStatusQueryHandler : IRequestHandler<GetOrdersByStatusQuery, List<OrderDto>>
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public GetOrdersByStatusQueryHandler(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<List<OrderDto>> Handle(GetOrdersByStatusQuery request, CancellationToken ct)
    {
        var orders = await _db.Orders
            .Include(o => o.Items)
            .Include(o => o.Payment)
            .Include(o => o.Shipment)
            .Where(o => o.Status == request.Status)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(ct);

        return _mapper.Map<List<OrderDto>>(orders);
    }
}
