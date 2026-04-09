using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderManagement.API.Domain.Entities;
using OrderManagement.API.Infrastructure.Data;
using Shared.Enums;

namespace OrderManagement.API.Application.Commands;

public class CreateShipmentCommandHandler : IRequestHandler<CreateShipmentCommand>
{
    private readonly AppDbContext _db;
    private readonly ILogger<CreateShipmentCommandHandler> _logger;

    public CreateShipmentCommandHandler(AppDbContext db,
        ILogger<CreateShipmentCommandHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task Handle(CreateShipmentCommand request, CancellationToken ct)
    {
        var order = await _db.Orders
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, ct);
        if (order is null) return;

        order.Status = OrderStatus.ShippingCreated;
        order.UpdatedAt = DateTime.UtcNow;

        order.Shipment = new ShipmentRecord
        {
            OrderId = order.Id,
            TrackingReference = request.TrackingReference,
            EstimatedDispatch = request.EstimatedDispatch
        };

        await _db.SaveChangesAsync(ct);

        _logger.LogInformation("Shipment created for order {OrderId} {EventType} tracking={TrackingRef}",
            request.OrderId, "CreateShipment", request.TrackingReference);
    }
}
