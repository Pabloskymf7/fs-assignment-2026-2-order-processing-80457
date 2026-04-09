using MassTransit;
using MediatR;
using OrderManagement.API.Application.Commands;
using Shared.Enums;
using Shared.Messages;

namespace OrderManagement.API.Application.Consumers;

public class PaymentApprovedConsumer : IConsumer<PaymentApproved>
{
    private readonly IMediator _mediator;
    private readonly ILogger<PaymentApprovedConsumer> _logger;

    public PaymentApprovedConsumer(IMediator mediator, ILogger<PaymentApprovedConsumer> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<PaymentApproved> context)
    {
        _logger.LogInformation("Payment approved for order {OrderId} transaction {TxId} {EventType}",
            context.Message.OrderId, context.Message.TransactionId, "PaymentApproved");

        await _mediator.Send(new UpdateOrderStatusCommand(
            context.Message.OrderId,
            OrderStatus.PaymentApproved,
            TransactionId: context.Message.TransactionId));
    }
}

public class PaymentRejectedConsumer : IConsumer<PaymentRejected>
{
    private readonly IMediator _mediator;
    private readonly ILogger<PaymentRejectedConsumer> _logger;

    public PaymentRejectedConsumer(IMediator mediator, ILogger<PaymentRejectedConsumer> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<PaymentRejected> context)
    {
        _logger.LogWarning("Payment rejected for order {OrderId}: {Reason} {EventType}",
            context.Message.OrderId, context.Message.Reason, "PaymentRejected");

        await _mediator.Send(new UpdateOrderStatusCommand(
            context.Message.OrderId,
            OrderStatus.PaymentFailed,
            FailureReason: context.Message.Reason));
    }
}
