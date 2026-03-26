using MassTransit;
using MediatR;
using OrderManagement.API.Application.Commands;
using Shared.Enums;
using Shared.Messages;

namespace OrderManagement.API.Application.Consumers;

public class ShippingResultConsumer :
    IConsumer<ShippingCreated>,
    IConsumer<ShippingFailed>
{
    private readonly IMediator _mediator;
    private readonly ILogger<ShippingResultConsumer> _logger;

    public ShippingResultConsumer(IMediator mediator,
        ILogger<ShippingResultConsumer> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ShippingCreated> context)
    {
        _logger.LogInformation("Shipping created for order {OrderId}, tracking {Ref}",
            context.Message.OrderId, context.Message.TrackingReference);

        await _mediator.Send(new UpdateOrderStatusCommand(
            context.Message.OrderId,
            OrderStatus.ShippingCreated,
            TrackingReference: context.Message.TrackingReference,
            EstimatedDispatch: context.Message.EstimatedDispatch));
    }

    public async Task Consume(ConsumeContext<ShippingFailed> context)
    {
        _logger.LogWarning("Shipping failed for order {OrderId}: {Reason}",
            context.Message.OrderId, context.Message.Reason);

        await _mediator.Send(new UpdateOrderStatusCommand(
            context.Message.OrderId,
            OrderStatus.Failed,
            FailureReason: context.Message.Reason));
    }
}