using MassTransit;
using MediatR;
using OrderManagement.API.Application.Commands;
using Shared.Enums;
using Shared.Messages;

namespace OrderManagement.API.Application.Consumers;

public class InventoryResultConsumer :
    IConsumer<InventoryConfirmed>,
    IConsumer<InventoryFailed>
{
    private readonly IMediator _mediator;
    private readonly ILogger<InventoryResultConsumer> _logger;

    public InventoryResultConsumer(IMediator mediator,
        ILogger<InventoryResultConsumer> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<InventoryConfirmed> context)
    {
        _logger.LogInformation("Inventory confirmed for order {OrderId}",
            context.Message.OrderId);

        await _mediator.Send(new UpdateOrderStatusCommand(
            context.Message.OrderId,
            OrderStatus.InventoryConfirmed));
    }

    public async Task Consume(ConsumeContext<InventoryFailed> context)
    {
        _logger.LogWarning("Inventory failed for order {OrderId}: {Reason}",
            context.Message.OrderId, context.Message.Reason);

        await _mediator.Send(new UpdateOrderStatusCommand(
            context.Message.OrderId,
            OrderStatus.InventoryFailed,
            FailureReason: context.Message.Reason));
    }
}