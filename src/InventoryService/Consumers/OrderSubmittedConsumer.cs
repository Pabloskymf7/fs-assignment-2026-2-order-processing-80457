using MassTransit;
using Microsoft.Extensions.Logging;
using Shared.Messages;

namespace InventoryService.Consumers;

public class OrderSubmittedConsumer : IConsumer<OrderSubmitted>
{
    private readonly ILogger<OrderSubmittedConsumer> _logger;
    private readonly IPublishEndpoint _bus;

    public OrderSubmittedConsumer(
        ILogger<OrderSubmittedConsumer> logger,
        IPublishEndpoint bus)
    {
        _logger = logger;
        _bus = bus;
    }

    public async Task Consume(ConsumeContext<OrderSubmitted> context)
    {
        var msg = context.Message;

        _logger.LogInformation(
            "Checking inventory for order {OrderId}, customer {CustomerId}, items {ItemCount}",
            msg.OrderId, msg.CustomerId, msg.Items.Count);

        await Task.Delay(500); // simular tiempo de procesamiento

        var hasStock = CheckStock(msg.Items);

        if (hasStock)
        {
            _logger.LogInformation(
                "Inventory confirmed for order {OrderId}", msg.OrderId);

            await _bus.Publish(new InventoryConfirmed(msg.OrderId));
        }
        else
        {
            _logger.LogWarning(
                "Inventory failed for order {OrderId} - insufficient stock", msg.OrderId);

            await _bus.Publish(new InventoryFailed(msg.OrderId, "Insufficient stock for one or more items"));
        }
    }

    private bool CheckStock(List<OrderItemMessage> items)
    {
        // Simular stock disponible
        // En producción real consultaría una BD de inventario
        foreach (var item in items)
        {
            // Simular que productos con ID par siempre tienen stock
            // y productos con ID impar tienen stock si cantidad <= 50
            if (item.ProductId % 2 != 0 && item.Quantity > 50)
                return false;
        }
        return true;
    }
}