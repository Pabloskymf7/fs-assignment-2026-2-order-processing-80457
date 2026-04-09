using MassTransit;
using MediatR;
using OrderManagement.API.Domain.Entities;
using OrderManagement.API.Infrastructure.Data;
using Shared.Messages;

namespace OrderManagement.API.Application.Commands;

public class CheckoutOrderCommandHandler : IRequestHandler<CheckoutOrderCommand, Guid>
{
    private readonly AppDbContext _db;
    private readonly IPublishEndpoint _bus;
    private readonly ILogger<CheckoutOrderCommandHandler> _logger;

    public CheckoutOrderCommandHandler(
        AppDbContext db,
        IPublishEndpoint bus,
        ILogger<CheckoutOrderCommandHandler> logger)
    {
        _db = db;
        _bus = bus;
        _logger = logger;
    }

    public async Task<Guid> Handle(CheckoutOrderCommand request, CancellationToken ct)
    {
        var order = new Order
        {
            CustomerId = request.CustomerId,
            TotalAmount = request.Items.Sum(i => i.Quantity * i.UnitPrice),
            Items = request.Items.Select(i => new OrderItem
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList()
        };

        _db.Orders.Add(order);
        await _db.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Order {OrderId} created for customer {CustomerId} with total {Total}",
            order.Id, order.CustomerId, order.TotalAmount);

        await _bus.Publish(new OrderSubmitted(
            order.Id,
            order.CustomerId,
            request.Items.Select(i => new OrderItemMessage(
                i.ProductId, i.ProductName, i.Quantity, i.UnitPrice)).ToList(),
            order.TotalAmount,
            order.CreatedAt
        ), ct);

        _logger.LogInformation("OrderSubmitted event published for {OrderId}", order.Id);

        return order.Id;
    }
}