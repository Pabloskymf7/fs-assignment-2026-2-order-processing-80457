using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderManagement.API.Domain.Entities;
using OrderManagement.API.Infrastructure.Data;
using Shared.Enums;

namespace OrderManagement.API.Application.Commands;

public class UpdateOrderStatusCommandHandler : IRequestHandler<UpdateOrderStatusCommand>
{
    private readonly AppDbContext _db;
    private readonly ILogger<UpdateOrderStatusCommandHandler> _logger;

    public UpdateOrderStatusCommandHandler(AppDbContext db,
        ILogger<UpdateOrderStatusCommandHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task Handle(UpdateOrderStatusCommand request, CancellationToken ct)
    {
        var order = await _db.Orders
            .Include(o => o.Payment)
            .Include(o => o.Shipment)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, ct);

        if (order is null)
        {
            _logger.LogWarning("Order {OrderId} not found for status update", request.OrderId);
            return;
        }

        order.Status = request.NewStatus;
        order.UpdatedAt = DateTime.UtcNow;
        order.FailureReason = request.FailureReason ?? order.FailureReason;

        if (request.TransactionId is not null)
        {
            order.Payment = new PaymentRecord
            {
                OrderId = order.Id,
                TransactionId = request.TransactionId,
                Approved = request.NewStatus == OrderStatus.PaymentApproved,
                Reason = request.FailureReason
            };
        }

        if (request.TrackingReference is not null)
        {
            order.Shipment = new ShipmentRecord
            {
                OrderId = order.Id,
                TrackingReference = request.TrackingReference,
                EstimatedDispatch = request.EstimatedDispatch ?? DateTime.UtcNow.AddDays(3)
            };
        }

        await _db.SaveChangesAsync(ct);

        _logger.LogInformation("Order {OrderId} status updated to {Status}",
            order.Id, request.NewStatus);
    }
}