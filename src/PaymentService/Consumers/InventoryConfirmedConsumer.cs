using MassTransit;
using Microsoft.Extensions.Logging;
using Shared.Messages;

namespace PaymentService.Consumers;

public class InventoryConfirmedConsumer : IConsumer<InventoryConfirmed>
{
    private readonly ILogger<InventoryConfirmedConsumer> _logger;
    private readonly IPublishEndpoint _bus;

    // Tarjetas de prueba que siempre se rechazan
    private static readonly HashSet<string> _blockedCards = new()
    {
        "4000000000000002",
        "4000000000000069",
        "4000000000000119"
    };

    public InventoryConfirmedConsumer(
        ILogger<InventoryConfirmedConsumer> logger,
        IPublishEndpoint bus)
    {
        _logger = logger;
        _bus = bus;
    }

    public async Task Consume(ConsumeContext<InventoryConfirmed> context)
    {
        var msg = context.Message;

        _logger.LogInformation(
            "Processing payment for order {OrderId}", msg.OrderId);

        await Task.Delay(700); // simular tiempo de procesamiento

        var (approved, transactionId, reason) = SimulatePayment(msg.OrderId);

        if (approved)
        {
            _logger.LogInformation(
                "Payment approved for order {OrderId}, transaction {TxId}",
                msg.OrderId, transactionId);

            await _bus.Publish(new PaymentApproved(msg.OrderId, transactionId!));
        }
        else
        {
            _logger.LogWarning(
                "Payment rejected for order {OrderId}: {Reason}",
                msg.OrderId, reason);

            await _bus.Publish(new PaymentRejected(msg.OrderId, reason!));
        }
    }

    private (bool Approved, string? TransactionId, string? Reason) SimulatePayment(Guid orderId)
    {
        // Rechazar aleatoriamente ~20% de los pagos
        var random = new Random();
        if (random.Next(1, 101) <= 20)
            return (false, null, "Payment declined by issuing bank");

        // Generar ID de transacción único
        var transactionId = $"TXN-{orderId.ToString()[..8].ToUpper()}-{random.Next(1000, 9999)}";
        return (true, transactionId, null);
    }
}