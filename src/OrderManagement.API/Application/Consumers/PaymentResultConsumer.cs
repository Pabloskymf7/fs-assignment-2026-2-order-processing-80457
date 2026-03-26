using MassTransit;
using MediatR;
using OrderManagement.API.Application.Commands;
using Shared.Enums;
using Shared.Messages;

namespace OrderManagement.API.Application.Consumers;

public class PaymentResultConsumer :
    IConsumer<PaymentApproved>,
    IConsumer<PaymentRejected>
{
    private readonly IMediator _mediator;
    private readonly ILogger<PaymentResultConsumer> _logger;

    public PaymentResultConsumer(IMediator mediator,
        ILogger<PaymentResultConsumer> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<PaymentApproved> context)
    {
        _logger.LogInformation("Payment approved for order {OrderId}, transaction {TxId}",
            context.Message.OrderId, context.Message.TransactionId);

        await _mediator.Send(new UpdateOrderStatusCommand(
            context.Message.OrderId,
            OrderStatus.PaymentApproved,
            TransactionId: context.Message.TransactionId));
    }

    public async Task Consume(ConsumeContext<PaymentRejected> context)
    {
        _logger.LogWarning("Payment rejected for order {OrderId}: {Reason}",
            context.Message.OrderId, context.Message.Reason);

        await _mediator.Send(new UpdateOrderStatusCommand(
            context.Message.OrderId,
            OrderStatus.PaymentFailed,
            FailureReason: context.Message.Reason));
    }
}