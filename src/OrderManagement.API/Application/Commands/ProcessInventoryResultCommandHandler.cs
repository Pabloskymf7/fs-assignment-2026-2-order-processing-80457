using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderManagement.API.Infrastructure.Data;
using Shared.Enums;

namespace OrderManagement.API.Application.Commands;

public class ProcessInventoryResultCommandHandler : IRequestHandler<ProcessInventoryResultCommand>
{
    private readonly AppDbContext _db;
    private readonly ILogger<ProcessInventoryResultCommandHandler> _logger;

    public ProcessInventoryResultCommandHandler(AppDbContext db,
        ILogger<ProcessInventoryResultCommandHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task Handle(ProcessInventoryResultCommand request, CancellationToken ct)
    {
        var order = await _db.Orders.FirstOrDefaultAsync(o => o.Id == request.OrderId, ct);
        if (order is null) return;

        order.Status = request.Confirmed ? OrderStatus.InventoryConfirmed : OrderStatus.InventoryFailed;
        order.FailureReason = request.Reason;
        order.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);

        _logger.LogInformation("Inventory result processed for order {OrderId} {EventType} confirmed={Confirmed}",
            request.OrderId, "ProcessInventoryResult", request.Confirmed);
    }
}
