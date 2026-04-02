using MassTransit;
using Microsoft.Extensions.Logging;
using Shared.Messages;

namespace ShippingService.Consumers;

public class PaymentApprovedConsumer : IConsumer<PaymentApproved>
{
    private readonly ILogger<PaymentApprovedConsumer> _logger;
    private readonly IPublishEndpoint _bus;

    public PaymentApprovedConsumer(
        ILogger<PaymentApprovedConsumer> logger,
        IPublishEndpoint bus)
    {
        _logger = logger;
        _bus = bus;
    }

    public async Task Consume(ConsumeContext<PaymentApproved> context)
    {
        var msg = context.Message;

        _logger.LogInformation(
            "Creating shipment for order {OrderId}", msg.OrderId);

        await Task.Delay(600); // simular tiempo de procesamiento

        var (success, trackingRef, estimatedDispatch, reason) = SimulateShipping(msg.OrderId);

        if (success)
        {
            _logger.LogInformation(
                "Shipment created for order {OrderId}, tracking {TrackingRef}, dispatch {Dispatch}",
                msg.OrderId, trackingRef, estimatedDispatch);

            await _bus.Publish(new ShippingCreated(
                msg.OrderId,
                trackingRef!,
                estimatedDispatch));
        }
        else
        {
            _logger.LogWarning(
                "Shipping failed for order {OrderId}: {Reason}",
                msg.OrderId, reason);

            await _bus.Publish(new ShippingFailed(msg.OrderId, reason!));
        }
    }

    private (bool Success, string? TrackingRef, DateTime EstimatedDispatch, string? Reason)
        SimulateShipping(Guid orderId)
    {
        var random = new Random();

        // Rechazar aleatoriamente ~5% de los envíos
        if (random.Next(1, 101) <= 5)
            return (false, null, DateTime.MinValue, "No courier available for this destination");

        // Generar referencia de tracking
        var carriers = new[] { "DHL", "FedEx", "UPS", "GLS" };
        var carrier = carriers[random.Next(carriers.Length)];
        var trackingRef = $"{carrier}-{orderId.ToString()[..8].ToUpper()}-{random.Next(10000, 99999)}";

        // Fecha estimada de entrega: entre 2 y 5 días
        var estimatedDispatch = DateTime.UtcNow.AddDays(random.Next(2, 6));

        return (true, trackingRef, estimatedDispatch, null);
    }
}