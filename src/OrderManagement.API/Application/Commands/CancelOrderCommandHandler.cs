using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderManagement.API.Infrastructure.Data;
using Shared.Enums;

namespace OrderManagement.API.Application.Commands;

public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, bool>
{
    private readonly AppDbContext _db;
    private readonly ILogger<CancelOrderCommandHandler> _logger;

    public CancelOrderCommandHandler(AppDbContext db, ILogger<CancelOrderCommandHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<bool> Handle(CancelOrderCommand request, CancellationToken ct)
    {
        var order = await _db.Orders.FirstOrDefaultAsync(o => o.Id == request.OrderId, ct);

        if (order is null)
        {
            _logger.LogWarning("Cancel attempted for non-existent order {OrderId}", request.OrderId);
            return false;
        }

        if (order.Status == OrderStatus.Completed || order.Status == OrderStatus.Failed)
        {
            _logger.LogWarning("Cannot cancel order {OrderId} with status {Status}",
                request.OrderId, order.Status);
            return false;
        }

        order.Status = OrderStatus.Failed;
        order.FailureReason = "Cancelled by user";
        order.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);

        _logger.LogInformation("Order {OrderId} cancelled successfully", request.OrderId);
        return true;
    }
}
