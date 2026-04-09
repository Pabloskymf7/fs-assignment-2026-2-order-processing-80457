using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderManagement.API.Infrastructure.Data;
using Shared.Enums;

namespace OrderManagement.API.Application.Queries;

public class GetDashboardSummaryQueryHandler : IRequestHandler<GetDashboardSummaryQuery, DashboardSummaryDto>
{
    private readonly AppDbContext _db;

    public GetDashboardSummaryQueryHandler(AppDbContext db)
    {
        _db = db;
    }

    public async Task<DashboardSummaryDto> Handle(GetDashboardSummaryQuery request, CancellationToken ct)
    {
        var orders = await _db.Orders.ToListAsync(ct);

        return new DashboardSummaryDto
        {
            TotalOrders = orders.Count,
            CompletedOrders = orders.Count(o => o.Status == OrderStatus.Completed),
            FailedOrders = orders.Count(o => o.Status == OrderStatus.Failed
                || o.Status == OrderStatus.PaymentFailed
                || o.Status == OrderStatus.InventoryFailed),
            PendingOrders = orders.Count(o => o.Status != OrderStatus.Completed
                && o.Status != OrderStatus.Failed
                && o.Status != OrderStatus.PaymentFailed
                && o.Status != OrderStatus.InventoryFailed),
            TotalRevenue = orders
                .Where(o => o.Status == OrderStatus.Completed)
                .Sum(o => o.TotalAmount)
        };
    }
}
