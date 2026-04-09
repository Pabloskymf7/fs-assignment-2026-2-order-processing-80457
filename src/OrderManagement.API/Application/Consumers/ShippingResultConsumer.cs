using MassTransit;
using MediatR;
using OrderManagement.API.Application.Commands;
using Shared.Enums;
using Shared.Messages;

namespace OrderManagement.API.Application.Consumers;

public class ShippingCreatedConsumer : IConsumer<ShippingCreated>
{
    private readonly IMediator _mediator;
    private readonly ILogger<ShippingCreatedConsumer> _logger;
    private readonly IPublishEndpoint _bus;

    public ShippingCreatedConsumer(IMediator mediator, ILogger<ShippingCreatedConsumer> logger, IPublishEndpoint bus)
    {
        _mediator = mediator;
        _logger = logger;
        _bus = bus;
    }

    public async Task Consume(ConsumeContext<ShippingCreated> context)
    {
        _logger.LogInformation("Shipping created for order {OrderId} tracking {Ref} {EventType}",
            context.Message.OrderId, context.Message.TrackingReference, "ShippingCreated");

        await _mediator.Send(new UpdateOrderStatusCommand(
            context.Message.OrderId,
            OrderStatus.ShippingCreated,
            TrackingReference: context.Message.TrackingReference,
            EstimatedDispatch: context.Message.EstimatedDispatch));

        await _bus.Publish(new OrderCompleted(context.Message.OrderId));

        await _mediator.Send(new UpdateOrderStatusCommand(
            context.Message.OrderId,
            OrderStatus.Completed));
    }
}

public class ShippingFailedConsumer : IConsumer<ShippingFailed>
{
    private readonly IMediator _mediator;
    private readonly ILogger<ShippingFailedConsumer> _logger;

    public ShippingFailedConsumer(IMediator mediator, ILogger<ShippingFailedConsumer> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ShippingFailed> context)
    {
        _logger.LogWarning("Shipping failed for order {OrderId}: {Reason} {EventType}",
            context.Message.OrderId, context.Message.Reason, "ShippingFailed");

        await _mediator.Send(new UpdateOrderStatusCommand(
            context.Message.OrderId,
            OrderStatus.Failed,
            FailureReason: context.Message.Reason));
    }
}
