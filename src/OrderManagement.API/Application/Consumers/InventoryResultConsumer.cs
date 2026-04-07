using MassTransit;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using OrderManagement.API.Application.Commands;
using OrderManagement.API.Domain.Entities;
using OrderManagement.API.Infrastructure.Data;
using Shared.Enums;
using Shared.Messages;

namespace OrderManagement.API.Application.Consumers;

public class InventoryResultConsumer :
    IConsumer<InventoryConfirmed>,
    IConsumer<InventoryFailed>
{
    private readonly IMediator _mediator;
    private readonly ILogger<InventoryResultConsumer> _logger;
    private readonly IServiceProvider _serviceProvider;

    public InventoryResultConsumer(IMediator mediator,
        ILogger<InventoryResultConsumer> logger,
        IServiceProvider serviceProvider)
    {
        _mediator = mediator;
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public async Task Consume(ConsumeContext<InventoryConfirmed> context)
    {
        _logger.LogInformation(
            "Inventory confirmed for order {OrderId} {EventType}" , context.Message.OrderId, "InventoryConfirmed", context.Message.OrderId);

        await _mediator.Send(new UpdateOrderStatusCommand(
            context.Message.OrderId,
            OrderStatus.InventoryConfirmed));

        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.InventoryRecords.Add(new InventoryRecord
        {
            OrderId = context.Message.OrderId,
            Confirmed = true
        });
        await db.SaveChangesAsync();
    }

    public async Task Consume(ConsumeContext<InventoryFailed> context)
    {
        _logger.LogWarning("Inventory failed for order {OrderId}: {Reason}",
            context.Message.OrderId, context.Message.Reason);

        await _mediator.Send(new UpdateOrderStatusCommand(
            context.Message.OrderId,
            OrderStatus.InventoryFailed,
            FailureReason: context.Message.Reason));

        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.InventoryRecords.Add(new InventoryRecord
        {
            OrderId = context.Message.OrderId,
            Confirmed = false,
            Reason = context.Message.Reason
        });
        await db.SaveChangesAsync();
    }
}

